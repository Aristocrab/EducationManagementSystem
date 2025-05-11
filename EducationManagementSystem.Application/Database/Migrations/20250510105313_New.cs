using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationManagementSystem.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class New : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedSubjectGroup_Lessons_LessonId",
                table: "SelectedSubjectGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_SelectedSubjectGroup_SelectedSubjectGroupId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedSubjectGroup",
                table: "SelectedSubjectGroup");

            migrationBuilder.RenameTable(
                name: "SelectedSubjectGroup",
                newName: "SelectedSubjectGroups");

            migrationBuilder.RenameIndex(
                name: "IX_SelectedSubjectGroup_LessonId",
                table: "SelectedSubjectGroups",
                newName: "IX_SelectedSubjectGroups_LessonId");

            migrationBuilder.AddColumn<int>(
                name: "MaxStudents",
                table: "SelectedSubjectGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "SelectedSubjectGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedSubjectGroups",
                table: "SelectedSubjectGroups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedSubjectGroups_SubjectId",
                table: "SelectedSubjectGroups",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedSubjectGroups_Lessons_LessonId",
                table: "SelectedSubjectGroups",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedSubjectGroups_Subjects_SubjectId",
                table: "SelectedSubjectGroups",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_SelectedSubjectGroups_SelectedSubjectGroupId",
                table: "Students",
                column: "SelectedSubjectGroupId",
                principalTable: "SelectedSubjectGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedSubjectGroups_Lessons_LessonId",
                table: "SelectedSubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedSubjectGroups_Subjects_SubjectId",
                table: "SelectedSubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_SelectedSubjectGroups_SelectedSubjectGroupId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedSubjectGroups",
                table: "SelectedSubjectGroups");

            migrationBuilder.DropIndex(
                name: "IX_SelectedSubjectGroups_SubjectId",
                table: "SelectedSubjectGroups");

            migrationBuilder.DropColumn(
                name: "MaxStudents",
                table: "SelectedSubjectGroups");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "SelectedSubjectGroups");

            migrationBuilder.RenameTable(
                name: "SelectedSubjectGroups",
                newName: "SelectedSubjectGroup");

            migrationBuilder.RenameIndex(
                name: "IX_SelectedSubjectGroups_LessonId",
                table: "SelectedSubjectGroup",
                newName: "IX_SelectedSubjectGroup_LessonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedSubjectGroup",
                table: "SelectedSubjectGroup",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Account = table.Column<string>(type: "TEXT", nullable: false),
                    AccountAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    AutoConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    MonobankPaymentId = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StudentId",
                table: "Payments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedSubjectGroup_Lessons_LessonId",
                table: "SelectedSubjectGroup",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_SelectedSubjectGroup_SelectedSubjectGroupId",
                table: "Students",
                column: "SelectedSubjectGroupId",
                principalTable: "SelectedSubjectGroup",
                principalColumn: "Id");
        }
    }
}
