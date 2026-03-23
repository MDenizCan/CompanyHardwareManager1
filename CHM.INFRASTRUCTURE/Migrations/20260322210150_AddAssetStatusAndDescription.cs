using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHM.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetStatusAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFaulty",
                table: "Assets");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Assets");

            migrationBuilder.AddColumn<bool>(
                name: "IsFaulty",
                table: "Assets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
