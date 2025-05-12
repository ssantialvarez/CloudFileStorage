using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFileStorage.Migrations
{
    /// <inheritdoc />
    public partial class added_content_type_field_file : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "contentType",
                table: "Files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contentType",
                table: "Files");
        }
    }
}
