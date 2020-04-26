using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SigmaGraduateProj.Migrations
{
    public partial class RenameCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchDates");

            migrationBuilder.CreateTable(
                name: "Currencies",
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
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.CreateTable(
                name: "CurrencyExchDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrencyCode = table.Column<int>(type: "integer", nullable: false),
                    CurrencyName = table.Column<string>(type: "text", nullable: true),
                    ExchangeDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchDates", x => x.Id);
                });
        }
    }
}
