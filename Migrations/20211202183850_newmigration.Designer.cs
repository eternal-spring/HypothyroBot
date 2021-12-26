﻿// <auto-generated />
using System;
using HypothyroBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HypothyroBot.Migrations
{
    [DbContext(typeof(UsersDataBaseContext))]
    [Migration("20211202183850_newmigration")]
    partial class newmigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HypothyroBot.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("Date");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mode")
                        .IsRequired()
                        .HasColumnType("nvarchar(24)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OperationDate")
                        .HasColumnType("Date");

                    b.Property<string>("Pathology")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)");

                    b.Property<double>("PretreatmentDose")
                        .HasColumnType("float");

                    b.Property<string>("PretreatmentDrug")
                        .IsRequired()
                        .HasColumnType("nvarchar(24)");

                    b.Property<double>("TSH")
                        .HasColumnType("float");

                    b.Property<DateTime>("TestDate")
                        .HasColumnType("Date");

                    b.Property<string>("ThyroidCondition")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)");

                    b.Property<double>("TreatmentDose")
                        .HasColumnType("float");

                    b.Property<string>("TreatmentDrug")
                        .IsRequired()
                        .HasColumnType("nvarchar(24)");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.Property<long>("checkinterval")
                        .HasColumnType("bigint");

                    b.Property<double>("lowpthslev")
                        .HasColumnType("float");

                    b.Property<double>("uppthslev")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
