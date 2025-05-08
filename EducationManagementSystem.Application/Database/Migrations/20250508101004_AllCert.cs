using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationManagementSystem.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class AllCert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_AllowedCertificates_AllowedCertificateId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_AllowedCertificateId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "AllowedCertificateId",
                table: "Subjects");

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "AllowedCertificates",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AllowedCertificates_SubjectId",
                table: "AllowedCertificates",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AllowedCertificates_Subjects_SubjectId",
                table: "AllowedCertificates",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllowedCertificates_Subjects_SubjectId",
                table: "AllowedCertificates");

            migrationBuilder.DropIndex(
                name: "IX_AllowedCertificates_SubjectId",
                table: "AllowedCertificates");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "AllowedCertificates");

            migrationBuilder.AddColumn<Guid>(
                name: "AllowedCertificateId",
                table: "Subjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_AllowedCertificateId",
                table: "Subjects",
                column: "AllowedCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_AllowedCertificates_AllowedCertificateId",
                table: "Subjects",
                column: "AllowedCertificateId",
                principalTable: "AllowedCertificates",
                principalColumn: "Id");
        }
    }
}
