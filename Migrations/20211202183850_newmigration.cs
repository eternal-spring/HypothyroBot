using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HypothyroBot.Migrations
{
    public partial class newmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkinterval",
                table: "Users");

            migrationBuilder.AddColumn<double>(
                name: "checkinterval",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 60.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkinterval",
                table: "Users");
            migrationBuilder.AddColumn<TimeSpan>(
                name: "checkinterval",
                table: "Users",
                type: "time",
                nullable: false,
                defaultValue: (0,0,0,0,0));
        }
    }
}