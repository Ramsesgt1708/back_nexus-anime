using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_bd.Migrations
{
    /// <inheritdoc />
    public partial class AnimeHistorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialVisualizaciones_Episodios_EpisodioId",
                table: "HistorialVisualizaciones");

            migrationBuilder.DropIndex(
                name: "IX_HistorialVisualizaciones_UsuarioId",
                table: "HistorialVisualizaciones");

            migrationBuilder.AddColumn<int>(
                name: "AnimeId",
                table: "HistorialVisualizaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // ✅ PASO 1: Poblar AnimeId desde la tabla Episodios
            migrationBuilder.Sql(@"
                UPDATE h
                SET h.AnimeId = e.AnimeId
                FROM HistorialVisualizaciones h
                INNER JOIN Episodios e ON h.EpisodioId = e._id
            ");

            // ✅ PASO 2: Eliminar duplicados (conservar el más reciente por UsuarioId + AnimeId)
            migrationBuilder.Sql(@"
                DELETE FROM HistorialVisualizaciones
                WHERE _id NOT IN (
                    SELECT MAX(_id)
                    FROM HistorialVisualizaciones
                    GROUP BY UsuarioId, AnimeId
                )
            ");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialVisualizaciones_AnimeId",
                table: "HistorialVisualizaciones",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialVisualizaciones_UsuarioId_AnimeId",
                table: "HistorialVisualizaciones",
                columns: new[] { "UsuarioId", "AnimeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialVisualizaciones_Animes_AnimeId",
                table: "HistorialVisualizaciones",
                column: "AnimeId",
                principalTable: "Animes",
                principalColumn: "_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialVisualizaciones_Episodios_EpisodioId",
                table: "HistorialVisualizaciones",
                column: "EpisodioId",
                principalTable: "Episodios",
                principalColumn: "_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialVisualizaciones_Animes_AnimeId",
                table: "HistorialVisualizaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialVisualizaciones_Episodios_EpisodioId",
                table: "HistorialVisualizaciones");

            migrationBuilder.DropIndex(
                name: "IX_HistorialVisualizaciones_AnimeId",
                table: "HistorialVisualizaciones");

            migrationBuilder.DropIndex(
                name: "IX_HistorialVisualizaciones_UsuarioId_AnimeId",
                table: "HistorialVisualizaciones");

            migrationBuilder.DropColumn(
                name: "AnimeId",
                table: "HistorialVisualizaciones");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialVisualizaciones_UsuarioId",
                table: "HistorialVisualizaciones",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialVisualizaciones_Episodios_EpisodioId",
                table: "HistorialVisualizaciones",
                column: "EpisodioId",
                principalTable: "Episodios",
                principalColumn: "_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
