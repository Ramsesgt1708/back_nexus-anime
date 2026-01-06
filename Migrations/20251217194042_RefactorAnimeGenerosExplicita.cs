using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_bd.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAnimeGenerosExplicita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimeGeneros_Animes_Animes_id",
                table: "AnimeGeneros");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimeGeneros_Generos_Generos_id",
                table: "AnimeGeneros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimeGeneros",
                table: "AnimeGeneros");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Episodios");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Planes",
                newName: "FechaRegistro");

            migrationBuilder.RenameColumn(
                name: "NumeroEpisodio",
                table: "Episodios",
                newName: "Numero");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Episodios",
                newName: "FechaRegistro");

            migrationBuilder.RenameColumn(
                name: "Generos_id",
                table: "AnimeGeneros",
                newName: "GeneroId");

            migrationBuilder.RenameColumn(
                name: "Animes_id",
                table: "AnimeGeneros",
                newName: "AnimeId");

            migrationBuilder.RenameIndex(
                name: "IX_AnimeGeneros_Generos_id",
                table: "AnimeGeneros",
                newName: "IX_AnimeGeneros_GeneroId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Planes",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Planes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "Planes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Anime_id",
                table: "Generos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "Episodios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Episodios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "_id",
                table: "AnimeGeneros",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimeGeneros",
                table: "AnimeGeneros",
                column: "_id");

            migrationBuilder.CreateTable(
                name: "Favoritos",
                columns: table => new
                {
                    _id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaAgregado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    AnimeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoritos", x => x._id);
                    table.ForeignKey(
                        name: "FK_Favoritos_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoritos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialVisualizaciones",
                columns: table => new
                {
                    _id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaVisualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EpisodioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialVisualizaciones", x => x._id);
                    table.ForeignKey(
                        name: "FK_HistorialVisualizaciones_Episodios_EpisodioId",
                        column: x => x.EpisodioId,
                        principalTable: "Episodios",
                        principalColumn: "_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialVisualizaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Generos_Anime_id",
                table: "Generos",
                column: "Anime_id");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeGeneros_AnimeId_GeneroId",
                table: "AnimeGeneros",
                columns: new[] { "AnimeId", "GeneroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_AnimeId",
                table: "Favoritos",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_UsuarioId_AnimeId",
                table: "Favoritos",
                columns: new[] { "UsuarioId", "AnimeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialVisualizaciones_EpisodioId",
                table: "HistorialVisualizaciones",
                column: "EpisodioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialVisualizaciones_UsuarioId",
                table: "HistorialVisualizaciones",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimeGeneros_Animes_AnimeId",
                table: "AnimeGeneros",
                column: "AnimeId",
                principalTable: "Animes",
                principalColumn: "_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnimeGeneros_Generos_GeneroId",
                table: "AnimeGeneros",
                column: "GeneroId",
                principalTable: "Generos",
                principalColumn: "_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Generos_Animes_Anime_id",
                table: "Generos",
                column: "Anime_id",
                principalTable: "Animes",
                principalColumn: "_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimeGeneros_Animes_AnimeId",
                table: "AnimeGeneros");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimeGeneros_Generos_GeneroId",
                table: "AnimeGeneros");

            migrationBuilder.DropForeignKey(
                name: "FK_Generos_Animes_Anime_id",
                table: "Generos");

            migrationBuilder.DropTable(
                name: "Favoritos");

            migrationBuilder.DropTable(
                name: "HistorialVisualizaciones");

            migrationBuilder.DropIndex(
                name: "IX_Generos_Anime_id",
                table: "Generos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimeGeneros",
                table: "AnimeGeneros");

            migrationBuilder.DropIndex(
                name: "IX_AnimeGeneros_AnimeId_GeneroId",
                table: "AnimeGeneros");

            migrationBuilder.DropColumn(
                name: "Anime_id",
                table: "Generos");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Episodios");

            migrationBuilder.DropColumn(
                name: "_id",
                table: "AnimeGeneros");

            migrationBuilder.RenameColumn(
                name: "FechaRegistro",
                table: "Planes",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "Numero",
                table: "Episodios",
                newName: "NumeroEpisodio");

            migrationBuilder.RenameColumn(
                name: "FechaRegistro",
                table: "Episodios",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "GeneroId",
                table: "AnimeGeneros",
                newName: "Generos_id");

            migrationBuilder.RenameColumn(
                name: "AnimeId",
                table: "AnimeGeneros",
                newName: "Animes_id");

            migrationBuilder.RenameIndex(
                name: "IX_AnimeGeneros_GeneroId",
                table: "AnimeGeneros",
                newName: "IX_AnimeGeneros_Generos_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Planes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Planes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "Planes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Planes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "Episodios",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Episodios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimeGeneros",
                table: "AnimeGeneros",
                columns: new[] { "Animes_id", "Generos_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_AnimeGeneros_Animes_Animes_id",
                table: "AnimeGeneros",
                column: "Animes_id",
                principalTable: "Animes",
                principalColumn: "_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnimeGeneros_Generos_Generos_id",
                table: "AnimeGeneros",
                column: "Generos_id",
                principalTable: "Generos",
                principalColumn: "_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
