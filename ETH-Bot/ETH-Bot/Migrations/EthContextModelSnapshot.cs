﻿// <auto-generated />
using ETH_Bot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ETHBot.Migrations
{
    [DbContext(typeof(EthContext))]
    partial class EthContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("ETH_Bot.Data.Entities.AlgDat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Href");

                    b.HasKey("Id");

                    b.ToTable("AlgDat");
                });

            modelBuilder.Entity("ETH_Bot.Data.Entities.DiscMath", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Href");

                    b.HasKey("Id");

                    b.ToTable("DiscMath");
                });

            modelBuilder.Entity("ETH_Bot.Data.Entities.Eprog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Href");

                    b.HasKey("Id");

                    b.ToTable("Eprog");
                });

            modelBuilder.Entity("ETH_Bot.Data.Entities.LinAlg", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Href");

                    b.HasKey("Id");

                    b.ToTable("LinAlg");
                });

            modelBuilder.Entity("ETH_Bot.Data.Entities.SubEntities.Reminder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Message");

                    b.Property<DateTime>("Time");

                    b.Property<ulong>("UserForeignId");

                    b.HasKey("Id");

                    b.HasIndex("UserForeignId");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("ETH_Bot.Data.Entities.User", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<bool>("Subscribed");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ETH_Bot.Data.Entities.SubEntities.Reminder", b =>
                {
                    b.HasOne("ETH_Bot.Data.Entities.User", "User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserForeignId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
