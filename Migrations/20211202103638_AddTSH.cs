using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HypothyroBot.Migrations
{
    public partial class AddTSH : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Histology",
                table: "Users",
                newName: "Pathology");

            migrationBuilder.RenameColumn(
                name: "DateOfOperation",
                table: "Users",
                newName: "TestDate");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "Users",
                newName: "OperationDate");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Users",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "TSH",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

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
                name: "BirthDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TSH",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "checkinterval",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TestDate",
                table: "Users",
                newName: "DateOfOperation");

            migrationBuilder.RenameColumn(
                name: "Pathology",
                table: "Users",
                newName: "Histology");

            migrationBuilder.RenameColumn(
                name: "OperationDate",
                table: "Users",
                newName: "DateOfBirth");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
