using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Real.Model;
using Toolbelt.ComponentModel.DataAnnotations;

namespace Real.Data.Contexts {

    public static partial class CapstoneContextExtensions {
        public static async Task<string> GetSettingAsync(this CapstoneContext context, AppSettingType setting) {
            return (await context.AppSettings.FirstOrDefaultAsync(x => x.AppSettingType == setting))?.Value;
        }

        public static async Task SetSettingAsync(this CapstoneContext context, AppSettingType setting, string value) {
            var item = await context.AppSettings.FirstOrDefaultAsync(x => x.AppSettingType == setting);
            
            if (item != null) {
                item.Value = value;
                context.Entry(item).State = EntityState.Modified;
            } else {
                item = new AppSetting {
                    AppSettingType = setting,
                    Setting = setting.ToString(),
                    Value = value
                };
                context.AppSettings.Add(item);
            }

            await context.SaveChangesAsync();
        }
    }

    public class CapstoneContext : DbContext {

        public string DatabaseName { get { return this.Database.GetDbConnection().Database; } }
        
        internal DbSet<AppSetting> AppSettings { get; set; }


        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyAnswer> SurveyAnswers { get; set; }
        public DbSet<Analytic> Analytics { get; set; }
        public DbSet<AnalyticError> AnalyticErrors { get; set; }
        public DbSet<AnalyticDetail> AnalyticDetails { get; set; }
        
        public DbSet<Location> Locations { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserGender> UserGenders { get; set; }
        public DbSet<UserBodyType> UserBodyTypes { get; set; }
        public DbSet<UserSurveyResponse> UserSurveyResponses { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<UserMatch> UserMatches { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }
        public DbSet<Email> Emails { get; set; }



        public DbSet<Precalc_Location> Precalc_Locations { get; set; }
        public DbSet<Precalc_ProfileMatch> Precalc_ProfileMatches { get; set; }
        



        [DbFunction("DistanceBetweenPoints")]
        public static double DistanceBetweenPoints(double lat1, double lon1, double lat2, double lon2) => throw new NotSupportedException();

        [DbFunction("Rand")]
        public static double Rand() => throw new NotSupportedException();


#region Information Schema

        public DbSet<Model.InformationSchema.Column> InformationSchema_Columns { get; set; }
        public DbSet<Model.InformationSchema.Table> InformationSchema_Tables { get; set; }

#endregion

        public CapstoneContext() {}
        public CapstoneContext(DbContextOptions options): base(options) {
            this.Database.EnsureCreated();
        }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            // should be configured already in Startup.cs
            if (optionsBuilder.IsConfigured) {
                base.OnConfiguring(optionsBuilder);
                return;
            }

            // The manual connection here is only for EF migrations.
            // Migrations uses the 

            var contentRoot = Directory.GetCurrentDirectory();

            if (!File.Exists(Path.Combine(contentRoot, "appsettings.json"))) {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                contentRoot = Path.GetDirectoryName(pathToExe);
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile($"appsettings.json", false)
                .AddJsonFile($"appsettings.Secrets.json", false)
                .AddJsonFile($"appsettings.Development.json", true)
                .AddUserSecrets("23855cc9-cfc1-404b-ad52-26b0d631595d");
            var config = builder.Build();

            var dns = config["aws:MySql:dns"]; // need to leave dns in secrets file because it's open to the public
            if (String.IsNullOrEmpty(dns)) {
                throw new InvalidProgramException("Unable to retrieve values from user secrets file.");
            }
            var user = config["aws:MySql:user"];
            var password = config["aws:MySql:password"];
            var databaseName = config["aws:MySql:databasename"];
            var connectionString = $"server={dns};user={user};password={password};database={databaseName}";

            optionsBuilder
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options => {
                    options.EnableRetryOnFailure(5);
                    options.CommandTimeout(Int32.MaxValue);
                })
                .UseLoggerFactory(LoggerFactory.Create(b => b
                    .AddConsole()
                    .AddFilter(level => level >= LogLevel.Warning)) // Information
                )
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.BuildIndexesFromAnnotations();

            modelBuilder.Entity<Real.Model.AnalyticError>()
                .HasOne(x => x.Analytic)
                .WithOne(x => x.AnalyticError)
                .HasForeignKey<AnalyticError>(x => x.Id);
            
            modelBuilder.Entity<Real.Model.AnalyticDetail>()
                .HasOne(x => x.Analytic)
                .WithOne(x => x.AnalyticDetail)
                .HasForeignKey<AnalyticDetail>(x => x.Id);

            modelBuilder.Entity<Real.Model.User>()
                .HasOne(x => x.UserGender)
                .WithMany(x => x.UserGenders)
                .HasForeignKey(x => x.UserGenderId);

            modelBuilder.Entity<Real.Model.User>()
                .HasMany(x => x.GendersAttractedTo)
                .WithMany(x => x.UserAttractedTo)
                .UsingEntity(x => x.ToTable("UserAttractedGenders"));

            modelBuilder.Entity<Real.Model.InformationSchema.Column>()
                .HasQueryFilter(x => x.TABLE_SCHEMA == DatabaseName);
                
            modelBuilder.Entity<Real.Model.InformationSchema.Table>()
                .HasQueryFilter(x => x.TABLE_SCHEMA == DatabaseName);


            modelBuilder.HasDbFunction(typeof(CapstoneContext).GetMethod(nameof(CapstoneContext.DistanceBetweenPoints), new [] { typeof(double),typeof(double),typeof(double),typeof(double), }))
                .HasName("DistanceBetweenPoints");

            modelBuilder.HasDbFunction(typeof(CapstoneContext).GetMethod(nameof(CapstoneContext.Rand)))
                .HasName("Rand");


            modelBuilder.Entity<UserBodyType>().HasData(
                new UserBodyType { Id = 1, Name = "Prefer not to say" },
                new UserBodyType { Id = 2, Name = "Skinny" },
                new UserBodyType { Id = 3, Name = "Average" },
                new UserBodyType { Id = 4, Name = "Muscular" },
                new UserBodyType { Id = 5, Name = "More to love" },
                new UserBodyType { Id = 6, Name = "Dadbod" }
            );

            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting {
                    Id = 1,
                    AppSettingType = AppSettingType.GPSRoundDecimalPlaces,
                    Setting =  AppSettingType.GPSRoundDecimalPlaces.ToString(),
                    Value = "4",
                }
            );

            modelBuilder.Entity<UserGender>().HasData(
                new UserGender { Id = 1, Name = "Male" },
                new UserGender { Id = 2, Name = "Female" },
                new UserGender { Id = 3, Name = "Other" }
            );

            modelBuilder.Entity<SurveyQuestion>().HasData(
                new SurveyQuestion {
                    Id = 1,
                    QuestionType = QuestionType.SingleChoice,
                    QuestionText = "What is your name?",
                },
                new SurveyQuestion {
                    Id = 2,
                    QuestionType = QuestionType.SingleChoice,
                    QuestionText = "What is your quest?",
                }
            );
            
            modelBuilder.Entity<SurveyAnswer>().HasData(
                new SurveyAnswer {
                    Id = 1,
                    SurveyQuestionId = 1,
                    AnswerText = "Arthur, king of the Britons",
                },
                new SurveyAnswer {
                    Id = 2,
                    SurveyQuestionId = 1,
                    AnswerText = "Al Gore, founder of the Internet",
                },
                new SurveyAnswer {
                    Id = 3,
                    SurveyQuestionId = 1,
                    AnswerText = "Bob, inventor of human suffering",
                },
                new SurveyAnswer {
                    Id = 4,
                    SurveyQuestionId = 2,
                    AnswerText = "I want tacos",
                },
                new SurveyAnswer {
                    Id = 5,
                    SurveyQuestionId = 2,
                    AnswerText = "I seek the grail",
                }
            );

            // base.OnModelCreating(modelBuilder);
        }
    }

}