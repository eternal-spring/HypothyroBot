using System;
using HypothyroBot.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HypothyroBot.Migrations
{
    public partial class m1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterDatabase()
            //    .Annotation("Npgsql:Enum:drug_type", "none,eutirox,l_thyroxine,another")
            //    .Annotation("Npgsql:Enum:gender_type", "none,male,female,unknown")
            //    .Annotation("Npgsql:Enum:mode_type", "adding_user,relevance_assessment,set_reminder,on_reminder,limitation_checking,results_collecting,control,user_data_correction")
            //    .Annotation("Npgsql:Enum:pathology_type", "none,nodular_non_toxic_goiter,diffuse_toxic_goiter,papillary_or_follicular_carcinoma,medullary_carcinoma,another")
            //    .Annotation("Npgsql:Enum:thyroid_type", "none,completely_removed,half_removed,isthmus_removed,lobe_remainder_left,unknown");

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        Mode = table.Column<ModeType>(type: "mode_type", nullable: false),
            //        Name = table.Column<string>(type: "text", nullable: true),
            //        DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
            //        Gender = table.Column<GenderType>(type: "gender_type", nullable: false),
            //        Weight = table.Column<double>(type: "double precision", nullable: false),
            //        PretreatmentDose = table.Column<double>(type: "double precision", nullable: false),
            //        PretreatmentDrug = table.Column<DrugType>(type: "drug_type", nullable: false),
            //        DateOfOperation = table.Column<DateTime>(type: "date", nullable: false),
            //        TreatmentDose = table.Column<double>(type: "double precision", nullable: false),
            //        TreatmentDrug = table.Column<DrugType>(type: "drug_type", nullable: false),
            //        ThyroidCondition = table.Column<ThyroidType>(type: "thyroid_type", nullable: false),
            //        Pathology = table.Column<PathologyType>(type: "pathology_type", nullable: false),
            //        lowTshLevel = table.Column<double>(type: "double precision", nullable: false),
            //        upTshLevel = table.Column<double>(type: "double precision", nullable: false),
            //        checkinterval = table.Column<double>(type: "double precision", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Tests",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "text", nullable: false),
            //        TshLevel = table.Column<double>(type: "double precision", nullable: false),
            //        TestDate = table.Column<DateTime>(type: "date", nullable: false),
            //        UserId = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Tests", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Tests_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Tests_UserId",
            //    table: "Tests",
            //    column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
