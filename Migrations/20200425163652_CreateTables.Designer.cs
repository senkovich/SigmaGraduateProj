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
    [Migration("20200425163652_CreateTables")]
    partial class CreateTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SigmaGraduateProj.Models.CurrencyExchDate", b =>
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

                    b.ToTable("CurrencyExchDates");
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
#pragma warning restore 612, 618
        }
    }
}
