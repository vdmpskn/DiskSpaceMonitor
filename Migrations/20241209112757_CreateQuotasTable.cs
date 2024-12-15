using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiskSpaceMonitor.Migrations
{
    /// <inheritdoc />
    public partial class CreateQuotasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Quotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FreeSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsageSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    GrowthTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotas_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotas_ServerId",
                table: "Quotas",
                column: "ServerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quotas");

        }
    }
}
