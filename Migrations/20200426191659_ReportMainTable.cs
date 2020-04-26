using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SigmaGraduateProj.Migrations
{
    public partial class ReportMainTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quarter = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    TotalSumUah = table.Column<decimal>(nullable: false),
                    TaxSumUah = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportTABs_ReportId",
                table: "ReportTABs",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTABs_Reports_ReportId",
                table: "ReportTABs",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportTABs_Reports_ReportId",
                table: "ReportTABs");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_ReportTABs_ReportId",
                table: "ReportTABs");
        }
    }
}
