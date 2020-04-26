using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SigmaGraduateProj.Migrations
{
    public partial class CreateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyExchDates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrencyCode = table.Column<int>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(nullable: false),
                    ExchangeDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HBCurrencies",
                columns: table => new
                {
                    CurrencyCode = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrencyFullName = table.Column<string>(nullable: true),
                    CurrencyName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HBCurrencies", x => x.CurrencyCode);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Sum = table.Column<decimal>(nullable: false),
                    CurrencyName = table.Column<string>(maxLength: 3, nullable: false),
                    Sender = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchDates");

            migrationBuilder.DropTable(
                name: "HBCurrencies");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
