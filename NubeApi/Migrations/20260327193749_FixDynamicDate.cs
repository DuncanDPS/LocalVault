using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NubeCasera.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "ID",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "FechaDeCreacion",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "ID",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "FechaDeCreacion",
                value: new DateTime(2026, 3, 27, 19, 33, 41, 742, DateTimeKind.Utc).AddTicks(2448));
        }
    }
}
