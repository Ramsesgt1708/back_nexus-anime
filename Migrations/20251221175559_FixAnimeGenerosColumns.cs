using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_bd.Migrations
{
    /// <inheritdoc />
    public partial class FixAnimeGenerosColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Generos_Animes_Anime_id",
                table: "Generos");

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
                name: "_id",
                table: "AnimeGeneros");

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
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimeGeneros",
                table: "AnimeGeneros",
                columns: new[] { "AnimeId", "GeneroId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimeGeneros",
                table: "AnimeGeneros");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Episodios");

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

            migrationBuilder.CreateIndex(
                name: "IX_Generos_Anime_id",
                table: "Generos",
                column: "Anime_id");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeGeneros_AnimeId_GeneroId",
                table: "AnimeGeneros",
                columns: new[] { "AnimeId", "GeneroId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Generos_Animes_Anime_id",
                table: "Generos",
                column: "Anime_id",
                principalTable: "Animes",
                principalColumn: "_id");
        }
    }
}
