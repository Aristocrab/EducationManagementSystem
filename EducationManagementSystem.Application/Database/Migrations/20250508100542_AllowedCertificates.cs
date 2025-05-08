using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationManagementSystem.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class AllowedCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AllowedCertificateId",
                table: "Subjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AllowedCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedCertificates", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_AllowedCertificates_AllowedCertificateId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "AllowedCertificates");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_AllowedCertificateId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "AllowedCertificateId",
                table: "Subjects");
        }
    }
}
