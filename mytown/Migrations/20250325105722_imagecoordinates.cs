using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class imagecoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BusservId",
                table: "BusinessProfiles",
                newName: "BusServId");

            migrationBuilder.RenameColumn(
                name: "BuscatId",
                table: "BusinessProfiles",
                newName: "BusCatId");

            migrationBuilder.AlterColumn<int>(
                name: "image_positiony",
                table: "BusinessProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<int>(
                name: "image_positionx",
                table: "BusinessProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AddColumn<string>(
                name: "Businesscategory_name",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Businessservice_name",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "zoom",
                table: "BusinessProfiles",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Businesscategory_name",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "Businessservice_name",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "zoom",
                table: "BusinessProfiles");

            migrationBuilder.RenameColumn(
                name: "BusServId",
                table: "BusinessProfiles",
                newName: "BusservId");

            migrationBuilder.RenameColumn(
                name: "BusCatId",
                table: "BusinessProfiles",
                newName: "BuscatId");

            migrationBuilder.AlterColumn<decimal>(
                name: "image_positiony",
                table: "BusinessProfiles",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "image_positionx",
                table: "BusinessProfiles",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
