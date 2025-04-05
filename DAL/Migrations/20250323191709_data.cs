using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           


            migrationBuilder.CreateTable(
                name: "TbChoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChoiceText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbChoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbChoice_TbQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "TbQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                   
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbChoice_QuestionId",
                table: "TbChoice",
                column: "QuestionId");

        

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "TbChoice");

            migrationBuilder.DropTable(
                name: "TbResult");

            migrationBuilder.DropTable(
                name: "TbQuestion");

            migrationBuilder.DropTable(
                name: "TbExam");
        }
    }
}
