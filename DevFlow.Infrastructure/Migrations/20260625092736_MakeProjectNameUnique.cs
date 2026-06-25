using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeProjectNameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_WorkspaceId_Name",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_WorkspaceId_Name",
                table: "Projects",
                columns: new[] { "WorkspaceId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_WorkspaceId_Name",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_WorkspaceId_Name",
                table: "Projects",
                columns: new[] { "WorkspaceId", "Name" });
        }
    }
}
