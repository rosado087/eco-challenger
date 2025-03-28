﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EcoChallenger.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EcoChallenger.Models.Challenge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaxProgress")
                        .HasColumnType("int");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Challenges");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Description 1",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 1",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Description 2",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 2",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Description 3",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 3",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 4,
                            Description = "Description 4",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 4",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 5,
                            Description = "Description 5",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 5",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 6,
                            Description = "Description 6",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 6",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 7,
                            Description = "Description 7",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 7",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 8,
                            Description = "Description 8",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 8",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 9,
                            Description = "Description 9",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 9",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 10,
                            Description = "Description 10",
                            MaxProgress = 0,
                            Points = 10,
                            Title = "Daily Challenge 10",
                            Type = "Daily"
                        },
                        new
                        {
                            Id = 11,
                            Description = "Description 1",
                            MaxProgress = 5,
                            Points = 100,
                            Title = "Weekly Challenge 1",
                            Type = "Weekly"
                        },
                        new
                        {
                            Id = 12,
                            Description = "Description 2",
                            MaxProgress = 7,
                            Points = 160,
                            Title = "Weekly Challenge 2",
                            Type = "Weekly"
                        },
                        new
                        {
                            Id = 13,
                            Description = "Description 3",
                            MaxProgress = 3,
                            Points = 60,
                            Title = "Weekly Challenge 3",
                            Type = "Weekly"
                        },
                        new
                        {
                            Id = 14,
                            Description = "Description 4",
                            MaxProgress = 5,
                            Points = 100,
                            Title = "Weekly Challenge 4",
                            Type = "Weekly"
                        },
                        new
                        {
                            Id = 15,
                            Description = "Description 5",
                            MaxProgress = 4,
                            Points = 80,
                            Title = "Weekly Challenge 5",
                            Type = "Weekly"
                        });
                });

            modelBuilder.Entity("EcoChallenger.Models.Friend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("FriendId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("EcoChallenger.Models.UserChallenges", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ChallengeId")
                        .HasColumnType("int");

                    b.Property<int>("Progress")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("WasConcluded")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.HasIndex("UserId");

                    b.ToTable("UserChallenges");
                });

            modelBuilder.Entity("Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Eco-Warrior"
                        },
                        new
                        {
                            Id = 2,
                            Name = "NatureLover"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Green Guru"
                        });
                });

            modelBuilder.Entity("TagUsers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("SelectedTag")
                        .HasColumnType("bit");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.HasIndex("UserId");

                    b.ToTable("TagUsers");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoogleToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "tester1@gmail.com",
                            IsAdmin = false,
                            Password = "ef797c8118f02dfb649607dd5d3f8c7623048c9c063d532cc95c5ed7a898a64f",
                            Points = 0,
                            Username = "Tester1"
                        },
                        new
                        {
                            Id = 2,
                            Email = "tester2@gmail.com",
                            IsAdmin = false,
                            Password = "ef797c8118f02dfb649607dd5d3f8c7623048c9c063d532cc95c5ed7a898a64f",
                            Points = 0,
                            Username = "Tester2"
                        },
                        new
                        {
                            Id = 3,
                            Email = "tester3@gmail.com",
                            IsAdmin = false,
                            Password = "ef797c8118f02dfb649607dd5d3f8c7623048c9c063d532cc95c5ed7a898a64f",
                            Points = 0,
                            Username = "Tester3"
                        });
                });

            modelBuilder.Entity("UserToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("EcoChallenger.Models.UserChallenges", b =>
                {
                    b.HasOne("EcoChallenger.Models.Challenge", "Challenge")
                        .WithMany()
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Challenge");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TagUsers", b =>
                {
                    b.HasOne("Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tag");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserToken", b =>
                {
                    b.HasOne("User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
