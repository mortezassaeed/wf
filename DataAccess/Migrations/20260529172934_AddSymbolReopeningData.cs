using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSymbolReopeningData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReopeningSymbolData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SymbolName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequestCount = table.Column<int>(type: "int", nullable: true),
                    ClosingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReopeningSymbolData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReopeningSymbolData_ProcessInstanceData_Id",
                        column: x => x.Id,
                        principalTable: "ProcessInstanceData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReopeningSymbolData");
        }
    }
}
