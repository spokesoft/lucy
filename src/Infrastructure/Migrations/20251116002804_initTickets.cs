using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lucy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sequence_Projects_ProjectId",
                table: "Sequence");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sequence",
                table: "Sequence");

            migrationBuilder.RenameTable(
                name: "Sequence",
                newName: "Sequences");

            migrationBuilder.RenameIndex(
                name: "IX_Sequence_ProjectId_Type",
                table: "Sequences",
                newName: "IX_Sequences_ProjectId_Type");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sequences",
                table: "Sequences",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<long>(type: "INTEGER", nullable: false),
                    StatusId = table.Column<long>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Key",
                table: "Tickets",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ProjectId",
                table: "Tickets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_StatusId",
                table: "Tickets",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sequences_Projects_ProjectId",
                table: "Sequences",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sequences_Projects_ProjectId",
                table: "Sequences");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sequences",
                table: "Sequences");

            migrationBuilder.RenameTable(
                name: "Sequences",
                newName: "Sequence");

            migrationBuilder.RenameIndex(
                name: "IX_Sequences_ProjectId_Type",
                table: "Sequence",
                newName: "IX_Sequence_ProjectId_Type");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sequence",
                table: "Sequence",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sequence_Projects_ProjectId",
                table: "Sequence",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
