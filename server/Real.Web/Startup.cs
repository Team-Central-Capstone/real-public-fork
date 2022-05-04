using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Real.Data.Contexts;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
using System.Net;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Real.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging.Console;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.HttpOverrides;
using Real.Web.Areas.API.Controllers;

namespace Real.Web {
    
    public class Startup {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment _env { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env) {
            var cr = env.ContentRootPath;
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, env.IsDevelopment())
                .AddJsonFile($"appsettings.Secrets.json", false, env.IsDevelopment()) // this file is not checked into Github. It needs to be manually updated and shared.
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, env.IsDevelopment())
                .AddUserSecrets("23855cc9-cfc1-404b-ad52-26b0d631595d");
            Configuration = builder.Build();
            _env = env;
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            // Design pattern: singleton
            // somtimes need to access services from other areas
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddHttpsRedirection(options => {});

            #region DbContexts

            var dns = Configuration["aws:MySql:dns"]; // need to leave dns in secrets file because it's open to the public
            if (String.IsNullOrEmpty(dns)) {
                throw new InvalidProgramException("Unable to retrieve values from user secrets file.");
            }
            var user = Configuration["aws:MySql:user"];
            var password = Configuration["aws:MySql:password"];
            var databaseName = Configuration["aws:MySql:databasename"];
            var timeout = Int32.Parse(Configuration["aws:MySql:timeout"]);
            var retryOnFailure = Int32.Parse(Configuration["aws:MySql:retryOnFailure"]);
            var mysqlConnectionString = $"server={dns};user={user};password={password};database={databaseName}";
            var hcConnectionString = $"server={dns};user={user};password={password};database={databaseName}_hc";

            // https://docs.microsoft.com/en-us/ef/core/querying/related-data/lazy#lazy-loading-with-proxies
            services.AddDbContext<CapstoneContext>(optionsBuilder => {
                    optionsBuilder.UseMySql(mysqlConnectionString, ServerVersion.AutoDetect(mysqlConnectionString), options => {
                        options.EnableRetryOnFailure(retryOnFailure);
                        options.CommandTimeout(timeout);
                    });

                    if (_env.IsDevelopment()) {
                        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole().AddFilter(level => level >= LogLevel.Information)));
                        optionsBuilder.EnableSensitiveDataLogging();
                        optionsBuilder.EnableDetailedErrors();
                    }
                }, 
                ServiceLifetime.Transient,
                ServiceLifetime.Transient
            );

            #endregion

            #region Authorization

            // https://dev.to/ivan_pesenti/firebase-authentication-net-5-29oi
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-6.0
            FirebaseApp.Create(new AppOptions {
                Credential = GoogleCredential.FromFile(Path.Combine(_env.ContentRootPath, $"appsettings.Secrets.Google.json")),
            });

            services
                .AddAuthentication(options => {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options => {
                    options.LoginPath = "/account/signin-google";
                    options.LogoutPath = "/account/signout-google";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = true;
                    options.Cookie.Name = "Real.Web";
                })
                .AddGoogle(options => {
                    // https://www.roundthecode.com/dotnet/how-to-add-google-authentication-to-a-asp-net-core-application
                    options.ClientId = Configuration["google:oauth:clientid"];
                    options.ClientSecret = Configuration["google:oauth:clientsecret"];
                })
                // .AddJwtBearer(opt => {
                //     opt.Authority = Configuration["Jwt:Firebase:ValidIssuer"];
                //     opt.TokenValidationParameters = new TokenValidationParameters {
                //         ValidateIssuer = true,
                //         ValidateAudience = true,
                //         ValidateLifetime = true,
                //         ValidateIssuerSigningKey = true,
                //         ValidIssuer = Configuration["Jwt:Firebase:ValidIssuer"],
                //         ValidAudience = Configuration["Jwt:Firebase:ValidAudience"]
                //     };
                // })
                ;

            #endregion

            #region MVC

            services.AddScoped<ApplicationAuthorizationFilter, ApplicationAuthorizationFilter>();

            var mvcBuilder = services.AddControllersWithViews(options => {
                options.Conventions.Add(new APINamespaceControllersConvention());
                options.UseDbLogging();

                if (!_env.IsDevelopment()) {
                    options.Filters.AddService<ApplicationAuthorizationFilter>();
                }

            })
            .AddRazorOptions(options => {
                
            })
            .AddControllersAsServices()
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });


            if (_env.IsDevelopment()) {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            #endregion

            #region Health Checks
            #if USE_HEALTHCHECKS

            services
                .AddHealthChecks()
                .AddDbContextCheck<CapstoneContext>(name: "Capstone connection")
            ;

            services
                .AddHealthChecksUI(options => {
                    options.SetEvaluationTimeInSeconds(60);
                    // options.MaximumHistoryEntriesPerEndpoint(60);
                    options.SetApiMaxActiveRequests(5);
                })
                .AddMySqlStorage(hcConnectionString, options => {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    
                })
            ;

            #endif
            #endregion

            #region Swagger

            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo { 
                    Version = "v1",
                    Title = "Real Dating API",
                    Description = "API Documentation for mobile app",
                    TermsOfService = new Uri("https://ccsu-sp2022-3md-capstone.us-east-2.elasticbeanstalk.com"),
                    Contact = new OpenApiContact {
                        Name = "Team 3MD Capstone",
                        Url = new Uri("https://ccsu-sp2022-3md-capstone.us-east-2.elasticbeanstalk.com"),
                    },
                    License = new OpenApiLicense {
                        Name = "License",
                        Url = new Uri("https://ccsu-sp2022-3md-capstone.us-east-2.elasticbeanstalk.com"),
                    }
                });

                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();
                options.EnableAnnotations(true, true);
                
                var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(x => x.FullName.StartsWith("Real.")).Select(x => x.Name);
                assemblies = assemblies.Concat(new[] { Assembly.GetExecutingAssembly().GetName().Name });
                foreach (var assembly in assemblies) {
                    var file = Path.Combine(AppContext.BaseDirectory, $"{assembly}.xml");
                    if (File.Exists(file))
                        options.IncludeXmlComments(file, true);
                }

                options.AddSecurityDefinition("UID", new OpenApiSecurityScheme {
                    Name = "UID",
                    Description = "",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                });
                options.AddSecurityDefinition("eUID", new OpenApiSecurityScheme {
                    Name = "eUID",
                    Description = "",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                });
                
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "UID"
                            }
                        },
                        new string[] {}
                    },
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "eUID"
                            }
                        },
                        new string[] {}
                    },
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            #endregion

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CapstoneContext capstoneContext) {

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            var pendingMigrations = capstoneContext.Database.GetPendingMigrations().Any();
            if (pendingMigrations) {
                capstoneContext.Database.Migrate();
            }

            app.UseCookiePolicy(new CookiePolicyOptions {
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.SameAsRequest,
                MinimumSameSitePolicy = SameSiteMode.Lax,
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            });

            app.UseStatusCodePages();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"); 
                options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                options.DefaultModelExpandDepth(2);
                options.DefaultModelsExpandDepth(2);
                options.ShowExtensions();
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                options.InjectStylesheet("/swagger-ui/custom.css");

            });

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "area_default",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "defaultRoute",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            
#if USE_HEALTHCHECKS

                endpoints.MapHealthChecks("/hc", new HealthCheckOptions {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    ResultStatusCodes = {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    AllowCachingResponses = false,
                });

                endpoints.MapHealthChecksUI(options => {
                    options.UIPath = "/health";
                    options.AsideMenuOpened = true;
                });
                
#endif

            });
            
        } // configure
    }
}
