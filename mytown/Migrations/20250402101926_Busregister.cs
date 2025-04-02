using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class Busregister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CnfPassword",
                table: "BusinessRegisters");

            migrationBuilder.DropColumn(
                name: "RegId",
                table: "BusinessRegisters");

            migrationBuilder.RenameColumn(
                name: "NewPassword",
                table: "BusinessRegisters",
                newName: "Password");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "BusinessRegisters",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BusinessVerification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationToken = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsVerified = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessVerification", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessVerification");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "BusinessRegisters");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "BusinessRegisters",
                newName: "NewPassword");

            migrationBuilder.AddColumn<string>(
                name: "CnfPassword",
                table: "BusinessRegisters",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RegId",
                table: "BusinessRegisters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
