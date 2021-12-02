using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HypothyroBot.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(24)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "Date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    PretreatmentDose = table.Column<double>(type: "float", nullable: false),
                    PretreatmentDrug = table.Column<string>(type: "nvarchar(24)", nullable: false),
                    DateOfOperation = table.Column<DateTime>(type: "Date", nullable: false),
                    TreatmentDose = table.Column<double>(type: "float", nullable: false),
                    TreatmentDrug = table.Column<string>(type: "nvarchar(24)", nullable: false),
                    ThyroidCondition = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Histology = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    lowpthslev = table.Column<double>(type: "float", nullable: false),
                    uppthslev = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
