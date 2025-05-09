using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationManagementSystem.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class Upd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Subjects_SubjectId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_SubjectId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Teachers");

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Groups",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubjectTeacher",
                columns: table => new
                {
                    SubjectsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeachersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTeacher", x => new { x.SubjectsId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_SubjectTeacher_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectTeacher_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TeacherId",
                table: "Groups",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectTeacher_TeachersId",
                table: "SubjectTeacher",
                column: "TeachersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Teachers_TeacherId",
                table: "Groups",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Teachers_TeacherId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "SubjectTeacher");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TeacherId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Groups");

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "Teachers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_SubjectId",
                table: "Teachers",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Subjects_SubjectId",
                table: "Teachers",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }
    }
}
