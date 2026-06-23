using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NubeCasera.Migrations
{
    /// <inheritdoc />
    public partial class AgregarThumbnailArchivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RutaThumbnail",
                table: "archivoReferencias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TieneThumbmail",
                table: "archivoReferencias",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RutaThumbnail",
                table: "archivoReferencias");

            migrationBuilder.DropColumn(
                name: "TieneThumbmail",
                table: "archivoReferencias");
        }
    }
}
