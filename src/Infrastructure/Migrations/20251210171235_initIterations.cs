using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lucy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initIterations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IterationId",
                table: "Tickets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Iterations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<long>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iterations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Iterations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IterationId",
                table: "Tickets",
                column: "IterationId");

            migrationBuilder.CreateIndex(
                name: "IX_Iterations_Key",
                table: "Iterations",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iterations_ProjectId",
                table: "Iterations",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Iterations_IterationId",
                table: "Tickets",
                column: "IterationId",
                principalTable: "Iterations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Iterations_IterationId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Iterations");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_IterationId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IterationId",
                table: "Tickets");
        }
    }
}
