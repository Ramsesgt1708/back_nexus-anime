using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_bd.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIsActiveAGenero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Generos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Generos");
        }
    }
}
