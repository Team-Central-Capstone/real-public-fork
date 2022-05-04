﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Real.Data.Contexts;

namespace Real.Data.Migrations
{
    [DbContext(typeof(CapstoneContext))]
    [Migration("20220129010149_analytics5")]
    partial class analytics5
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.11");

            modelBuilder.Entity("Real.Model.Analytic", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)");

                    b.Property<string>("Action")
                        .HasColumnType("longtext");

                    b.Property<string>("Area")
                        .HasColumnType("longtext");

                    b.Property<string>("Controller")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("EndTimestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FirebaseUserId")
                        .HasColumnType("longtext");

                    b.Property<string>("Host")
                        .HasColumnType("longtext");

                    b.Property<string>("IPv4")
                        .HasColumnType("longtext");

                    b.Property<string>("IPv6")
                        .HasColumnType("longtext");

                    b.Property<string>("Namespace")
                        .HasColumnType("longtext");

                    b.Property<string>("Path")
                        .HasColumnType("longtext");

                    b.Property<string>("QueryString")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("StartTimestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Analytics");
                });

            modelBuilder.Entity("Real.Model.InformationSchema.Column", b =>
                {
                    b.Property<long?>("CHARACTER_MAXIMUM_LENGTH")
                        .HasColumnType("bigint");

                    b.Property<long?>("CHARACTER_OCTET_LENGTH")
                        .HasColumnType("bigint");

                    b.Property<string>("CHARACTER_SET_NAME")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("COLLATION_NAME")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("COLUMN_COMMENT")
                        .HasMaxLength(65535)
                        .HasColumnType("longtext");

                    b.Property<string>("COLUMN_DEFAULT")
                        .HasMaxLength(65535)
                        .HasColumnType("longtext");

                    b.Property<string>("COLUMN_KEY")
                        .HasMaxLength(3)
                        .HasColumnType("varchar(3)");

                    b.Property<string>("COLUMN_NAME")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("COLUMN_TYPE")
                        .HasMaxLength(16777215)
                        .HasColumnType("longtext");

                    b.Property<string>("DATA_TYPE")
                        .HasColumnType("longtext");

                    b.Property<int?>("DATETIME_PRECISION")
                        .HasColumnType("int");

                    b.Property<string>("EXTRA")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("GENERATION_EXPRESSION")
                        .HasColumnType("longtext");

                    b.Property<string>("IS_NULLABLE")
                        .HasMaxLength(3)
                        .HasColumnType("varchar(3)");

                    b.Property<long?>("NUMERIC_PRECISION")
                        .HasColumnType("bigint");

                    b.Property<long?>("NUMERIC_SCALE")
                        .HasColumnType("bigint");

                    b.Property<int>("ORDINAL_POSITION")
                        .HasColumnType("int");

                    b.Property<string>("PRIVILEGES")
                        .HasMaxLength(154)
                        .HasColumnType("varchar(154)");

                    b.Property<int?>("SRS_ID")
                        .HasColumnType("int");

                    b.Property<string>("TABLE_CATALOG")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("TABLE_NAME")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("TABLE_SCHEMA")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.ToTable("Capstone_Information_Schema_Columns");
                });

            modelBuilder.Entity("Real.Model.InformationSchema.Table", b =>
                {
                    b.Property<long?>("AUTO_INCREMENT")
                        .HasColumnType("bigint");

                    b.Property<long?>("AVG_ROW_LENGTH")
                        .HasColumnType("bigint");

                    b.Property<long?>("CHECKSUM")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CHECK_TIME")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CREATE_OPTIONS")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<DateTime>("CREATE_TIME")
                        .HasColumnType("datetime(6)");

                    b.Property<long?>("DATA_FREE")
                        .HasColumnType("bigint");

                    b.Property<long?>("DATA_LENGTH")
                        .HasColumnType("bigint");

                    b.Property<string>("ENGINE")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<long?>("INDEX_LENGTH")
                        .HasColumnType("bigint");

                    b.Property<long?>("MAX_DATA_LENGTH")
                        .HasColumnType("bigint");

                    b.Property<string>("ROW_FORMAT")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("TABLE_CATALOG")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("TABLE_COLLATION")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("TABLE_COMMENT")
                        .HasMaxLength(65535)
                        .HasColumnType("longtext");

                    b.Property<string>("TABLE_NAME")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<long?>("TABLE_ROWS")
                        .HasColumnType("bigint");

                    b.Property<string>("TABLE_SCHEMA")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("TABLE_TYPE")
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11)");

                    b.Property<DateTime?>("UPDATE_TIME")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("VERSION")
                        .HasColumnType("int");

                    b.ToTable("Capstone_Information_Schema_Tables");
                });

            modelBuilder.Entity("Real.Model.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FirebaseUserId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<double>("Latitude")
                        .HasColumnType("double");

                    b.Property<double>("Longitude")
                        .HasColumnType("double");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("FirebaseUserId", "Timestamp", "Latitude", "Longitude")
                        .HasDatabaseName("IX_Locations");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Real.Model.SurveyAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AnswerText")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<int>("SurveyQuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SurveyQuestionId");

                    b.ToTable("SurveyAnswers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AnswerText = "Arthur, king of the Britons",
                            SurveyQuestionId = 1
                        },
                        new
                        {
                            Id = 2,
                            AnswerText = "Al Gore, founder of the Internet",
                            SurveyQuestionId = 1
                        },
                        new
                        {
                            Id = 3,
                            AnswerText = "Bob, inventor of human suffering",
                            SurveyQuestionId = 1
                        },
                        new
                        {
                            Id = 4,
                            AnswerText = "I want tacos",
                            SurveyQuestionId = 2
                        },
                        new
                        {
                            Id = 5,
                            AnswerText = "I seek the grail",
                            SurveyQuestionId = 2
                        });
                });

            modelBuilder.Entity("Real.Model.SurveyQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("QuestionText")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<int>("QuestionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SurveyQuestions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Order = 0,
                            QuestionText = "What is your name?",
                            QuestionType = 2
                        },
                        new
                        {
                            Id = 2,
                            Order = 0,
                            QuestionText = "What is your quest?",
                            QuestionType = 2
                        });
                });

            modelBuilder.Entity("Real.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FirebaseUserId")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("LastLoginTimestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PreferredName")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<DateTime?>("ProfileLastUpdatedTimestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("RegisteredTimestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UserGenderId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserGenderId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Real.Model.UserBlock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("BlockedOnDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("BlockedUserId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BlockedUserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserBlocks");
                });

            modelBuilder.Entity("Real.Model.UserGender", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.HasKey("Id");

                    b.ToTable("UserGenders");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Male"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Female"
                        });
                });

            modelBuilder.Entity("Real.Model.UserImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ContentType")
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FileName")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("longblob");

                    b.Property<bool>("IsProfilePhoto")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserImages");
                });

            modelBuilder.Entity("Real.Model.UserMatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("MatchedLatitude")
                        .HasColumnType("double");

                    b.Property<double>("MatchedLongitude")
                        .HasColumnType("double");

                    b.Property<DateTime>("MatchedOnDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("User1AcceptedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("User2AcceptedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId1")
                        .HasColumnType("int");

                    b.Property<int>("UserId2")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId1");

                    b.HasIndex("UserId2");

                    b.ToTable("UserMatches");
                });

            modelBuilder.Entity("Real.Model.UserMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("longtext");

                    b.Property<bool>("MessageRead")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId1")
                        .HasColumnType("int");

                    b.Property<int>("UserId2")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId1");

                    b.HasIndex("UserId2");

                    b.ToTable("UserMessages");
                });

            modelBuilder.Entity("Real.Model.UserSurveyResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("SurveyAnswerResponse")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<int>("SurveyQuestionId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("UserSurveyResponseWeight")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SurveyQuestionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSurveyResponses");
                });

            modelBuilder.Entity("SurveyAnswerUserSurveyResponse", b =>
                {
                    b.Property<int>("SurveyAnswersId")
                        .HasColumnType("int");

                    b.Property<int>("UserSurveyResponsesId")
                        .HasColumnType("int");

                    b.HasKey("SurveyAnswersId", "UserSurveyResponsesId");

                    b.HasIndex("UserSurveyResponsesId");

                    b.ToTable("SurveyAnswerUserSurveyResponse");
                });

            modelBuilder.Entity("Real.Model.SurveyAnswer", b =>
                {
                    b.HasOne("Real.Model.SurveyQuestion", null)
                        .WithMany("Answers")
                        .HasForeignKey("SurveyQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Real.Model.User", b =>
                {
                    b.HasOne("Real.Model.UserGender", "UserGender")
                        .WithMany()
                        .HasForeignKey("UserGenderId");

                    b.Navigation("UserGender");
                });

            modelBuilder.Entity("Real.Model.UserBlock", b =>
                {
                    b.HasOne("Real.Model.User", "BlockedUser")
                        .WithMany()
                        .HasForeignKey("BlockedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Real.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BlockedUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Real.Model.UserImage", b =>
                {
                    b.HasOne("Real.Model.User", "User")
                        .WithMany("UserImages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Real.Model.UserMatch", b =>
                {
                    b.HasOne("Real.Model.User", "User1")
                        .WithMany()
                        .HasForeignKey("UserId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Real.Model.User", "User2")
                        .WithMany()
                        .HasForeignKey("UserId2")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("Real.Model.UserMessage", b =>
                {
                    b.HasOne("Real.Model.User", "User1")
                        .WithMany()
                        .HasForeignKey("UserId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Real.Model.User", "User2")
                        .WithMany()
                        .HasForeignKey("UserId2")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("Real.Model.UserSurveyResponse", b =>
                {
                    b.HasOne("Real.Model.SurveyQuestion", "SurveyQuestion")
                        .WithMany()
                        .HasForeignKey("SurveyQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Real.Model.User", "User")
                        .WithMany("UserSurveyResponses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SurveyQuestion");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SurveyAnswerUserSurveyResponse", b =>
                {
                    b.HasOne("Real.Model.SurveyAnswer", null)
                        .WithMany()
                        .HasForeignKey("SurveyAnswersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Real.Model.UserSurveyResponse", null)
                        .WithMany()
                        .HasForeignKey("UserSurveyResponsesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Real.Model.SurveyQuestion", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("Real.Model.User", b =>
                {
                    b.Navigation("UserImages");

                    b.Navigation("UserSurveyResponses");
                });
#pragma warning restore 612, 618
        }
    }
}
