using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NubeCasera.Migrations
{
    /// <inheritdoc />
    public partial class makecarpetalogicaopcional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_archivoReferencias_categorias_carpetaLogicaID",
                table: "archivoReferencias");

            migrationBuilder.AlterColumn<Guid>(
                name: "carpetaLogicaID",
                table: "archivoReferencias",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_archivoReferencias_categorias_carpetaLogicaID",
                table: "archivoReferencias",
                column: "carpetaLogicaID",
                principalTable: "categorias",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_archivoReferencias_categorias_carpetaLogicaID",
                table: "archivoReferencias");

            migrationBuilder.AlterColumn<Guid>(
                name: "carpetaLogicaID",
                table: "archivoReferencias",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_archivoReferencias_categorias_carpetaLogicaID",
                table: "archivoReferencias",
                column: "carpetaLogicaID",
                principalTable: "categorias",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
