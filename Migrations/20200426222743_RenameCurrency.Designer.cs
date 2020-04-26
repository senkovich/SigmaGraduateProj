﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SigmaGraduateProj;

namespace SigmaGraduateProj.Migrations
{
    [DbContext(typeof(TransactionsDBContext))]
    [Migration("20200426222743_RenameCurrency")]
    partial class RenameCurrency
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SigmaGraduateProj.Models.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CurrencyCode")
                        .HasColumnType("integer");

                    b.Property<string>("CurrencyName")
                        .HasColumnType("text");

                    b.Property<DateTime>("ExchangeDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("SigmaGraduateProj.Models.HBCurrency", b =>
                {
                    b.Property<int>("CurrencyCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CurrencyFullName")
                        .HasColumnType("text");

                    b.Property<string>("CurrencyName")
                        .HasColumnType("text");

                    b.HasKey("CurrencyCode");

                    b.ToTable("HBCurrencies");
                });

            modelBuilder.Entity("SigmaGraduateProj.Models.Report", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Quarter")
                        .HasColumnType("integer");

                    b.Property<decimal>("TaxSumUah")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TotalSumUah")
                        .HasColumnType("numeric");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("SigmaGraduateProj.Models.ReportTAB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<decimal>("CurrencyExchRate")
                        .HasColumnType("numeric");

                    b.Property<string>("CurrencyName")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ReportId")
                        .HasColumnType("integer");

                    b.Property<string>("Sender")
                        .HasColumnType("text");

                    b.Property<decimal>("Sum")
                        .HasColumnType("numeric");

                    b.Property<decimal>("SumUah")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("ReportId");

                    b.ToTable("ReportTABs");
                });

            modelBuilder.Entity("SigmaGraduateProj.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<string>("CurrencyName")
                        .IsRequired()
                        .HasColumnType("character varying(3)")
                        .HasMaxLength(3);

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Sender")
                        .HasColumnType("text");

                    b.Property<decimal>("Sum")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("SigmaGraduateProj.Models.ReportTAB", b =>
                {
                    b.HasOne("SigmaGraduateProj.Models.Report", null)
                        .WithMany("Tab")
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
