using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationManagementSystem.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class SchoolName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Subjects_SubjectId",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Schools",
                newName: "SchoolName");

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedSubjectGroupId",
                table: "Students",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "Lessons",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelectedSubject",
                table: "Lessons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LessonId",
                table: "Groups",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SelectedSubjectGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LessonId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedSubjectGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedSubjectGroup_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_SelectedSubjectGroupId",
                table: "Students",
                column: "SelectedSubjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LessonId",
                table: "Groups",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedSubjectGroup_LessonId",
                table: "SelectedSubjectGroup",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Lessons_LessonId",
                table: "Groups",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Subjects_SubjectId",
                table: "Lessons",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_SelectedSubjectGroup_SelectedSubjectGroupId",
                table: "Students",
                column: "SelectedSubjectGroupId",
                principalTable: "SelectedSubjectGroup",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Lessons_LessonId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Subjects_SubjectId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_SelectedSubjectGroup_SelectedSubjectGroupId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "SelectedSubjectGroup");

            migrationBuilder.DropIndex(
                name: "IX_Students_SelectedSubjectGroupId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Groups_LessonId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SelectedSubjectGroupId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsSelectedSubject",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "SchoolName",
                table: "Schools",
                newName: "Balance");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "Lessons",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Subjects_SubjectId",
                table: "Lessons",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }
    }
}
