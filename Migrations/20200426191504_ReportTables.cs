using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SigmaGraduateProj.Migrations
{
    public partial class ReportTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportTABs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReportId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Sum = table.Column<decimal>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    CurrencyExchRate = table.Column<decimal>(nullable: false),
                    SumUah = table.Column<decimal>(nullable: false),
                    Sender = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTABs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportTABs");
        }
    }
}
