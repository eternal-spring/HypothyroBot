using Microsoft.EntityFrameworkCore.Migrations;

namespace HypothyroBot.Migrations
{
    public partial class M2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Actual",
                table: "Tests",
                type: "boolean",
                nullable: false,
                defaultValueSql: "true");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actual",
                table: "Tests");
        }
    }
}
