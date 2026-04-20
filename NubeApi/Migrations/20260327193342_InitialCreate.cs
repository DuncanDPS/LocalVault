using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NubeCasera.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    NombreCategoria = table.Column<string>(type: "TEXT", nullable: false),
                    CategoriaPadreID = table.Column<Guid>(type: "TEXT", nullable: true),
                    FechaDeCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.ID);
                    table.ForeignKey(
                        name: "FK_categorias_categorias_CategoriaPadreID",
                        column: x => x.CategoriaPadreID,
                        principalTable: "categorias",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "archivoReferencias",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    FechaDeSubida = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    TipoHash = table.Column<string>(type: "TEXT", nullable: false),
                    RutaDeAlmacenamiento = table.Column<string>(type: "TEXT", nullable: false),
                    Extension = table.Column<string>(type: "TEXT", nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", nullable: false),
                    TamanioBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    EstaEliminado = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaDeEliminacion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    carpetaLogicaID = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_archivoReferencias", x => x.ID);
                    table.ForeignKey(
                        name: "FK_archivoReferencias_categorias_carpetaLogicaID",
                        column: x => x.carpetaLogicaID,
                        principalTable: "categorias",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "categorias",
                columns: new[] { "ID", "CategoriaPadreID", "FechaDeCreacion", "NombreCategoria" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 3, 27, 19, 33, 41, 742, DateTimeKind.Utc).AddTicks(2448), "Principal" });

            migrationBuilder.CreateIndex(
                name: "IX_archivoReferencias_carpetaLogicaID",
                table: "archivoReferencias",
                column: "carpetaLogicaID");

            migrationBuilder.CreateIndex(
                name: "IX_archivoReferencias_EstaEliminado",
                table: "archivoReferencias",
                column: "EstaEliminado");

            migrationBuilder.CreateIndex(
                name: "IX_archivoReferencias_FechaDeEliminacion",
                table: "archivoReferencias",
                column: "FechaDeEliminacion");

            migrationBuilder.CreateIndex(
                name: "IX_categorias_CategoriaPadreID",
                table: "categorias",
                column: "CategoriaPadreID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "archivoReferencias");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
