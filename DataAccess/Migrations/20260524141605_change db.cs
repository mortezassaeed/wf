using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessStepActions_ProcessSteps_ProcessStepId",
                table: "ProcessStepActions");

            migrationBuilder.DropTable(
                name: "ProcessStepTransitions");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStepActions_ProcessStepId_Action",
                table: "ProcessStepActions");

            migrationBuilder.RenameColumn(
                name: "ProcessStepId",
                table: "ProcessStepActions",
                newName: "ToStepId");

            migrationBuilder.AddColumn<int>(
                name: "FromStepId",
                table: "ProcessStepActions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessId",
                table: "ProcessStepActions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStepActions_FromStepId",
                table: "ProcessStepActions",
                column: "FromStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStepActions_ProcessId",
                table: "ProcessStepActions",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStepActions_ToStepId",
                table: "ProcessStepActions",
                column: "ToStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessStepActions_ProcessSteps_FromStepId",
                table: "ProcessStepActions",
                column: "FromStepId",
                principalTable: "ProcessSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessStepActions_ProcessSteps_ToStepId",
                table: "ProcessStepActions",
                column: "ToStepId",
                principalTable: "ProcessSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessStepActions_Processes_ProcessId",
                table: "ProcessStepActions",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessStepActions_ProcessSteps_FromStepId",
                table: "ProcessStepActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessStepActions_ProcessSteps_ToStepId",
                table: "ProcessStepActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessStepActions_Processes_ProcessId",
                table: "ProcessStepActions");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStepActions_FromStepId",
                table: "ProcessStepActions");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStepActions_ProcessId",
                table: "ProcessStepActions");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStepActions_ToStepId",
                table: "ProcessStepActions");

            migrationBuilder.DropColumn(
                name: "FromStepId",
                table: "ProcessStepActions");

            migrationBuilder.DropColumn(
                name: "ProcessId",
                table: "ProcessStepActions");

            migrationBuilder.RenameColumn(
                name: "ToStepId",
                table: "ProcessStepActions",
                newName: "ProcessStepId");

            migrationBuilder.CreateTable(
                name: "ProcessStepTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromStepId = table.Column<int>(type: "int", nullable: false),
                    ProcessStepActionId = table.Column<int>(type: "int", nullable: false),
                    ToStepId = table.Column<int>(type: "int", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStepTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessStepTransitions_ProcessStepActions_ProcessStepActionId",
                        column: x => x.ProcessStepActionId,
                        principalTable: "ProcessStepActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessStepTransitions_ProcessSteps_FromStepId",
                        column: x => x.FromStepId,
                        principalTable: "ProcessSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessStepTransitions_ProcessSteps_ToStepId",
                        column: x => x.ToStepId,
                        principalTable: "ProcessSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStepActions_ProcessStepId_Action",
                table: "ProcessStepActions",
                columns: new[] { "ProcessStepId", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStepTransitions_FromStepId",
                table: "ProcessStepTransitions",
                column: "FromStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStepTransitions_ProcessStepActionId",
                table: "ProcessStepTransitions",
                column: "ProcessStepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStepTransitions_ToStepId",
                table: "ProcessStepTransitions",
                column: "ToStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessStepActions_ProcessSteps_ProcessStepId",
                table: "ProcessStepActions",
                column: "ProcessStepId",
                principalTable: "ProcessSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
