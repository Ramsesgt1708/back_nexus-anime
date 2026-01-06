using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_bd.Migrations
{
    /// <inheritdoc />
    public partial class TimeStampHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completado",
                table: "HistorialVisualizaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TiempoReproducido",
                table: "HistorialVisualizaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completado",
                table: "HistorialVisualizaciones");

            migrationBuilder.DropColumn(
                name: "TiempoReproducido",
                table: "HistorialVisualizaciones");
        }
    }
}
