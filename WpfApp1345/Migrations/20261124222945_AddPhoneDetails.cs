using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfApp1345.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Definition",
                table: "Phones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Phones",
                type: "text",
                nullable: false,
                defaultValue: "No image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Definition",
                table: "Phones");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Phones");
        }
    }
}
