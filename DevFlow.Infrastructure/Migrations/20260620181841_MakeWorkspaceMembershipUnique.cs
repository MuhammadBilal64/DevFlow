using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeWorkspaceMembershipUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkspacesMembers_UserId_WorkspaceId",
                table: "WorkspacesMembers");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspacesMembers_UserId_WorkspaceId",
                table: "WorkspacesMembers",
                columns: new[] { "UserId", "WorkspaceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkspacesMembers_UserId_WorkspaceId",
                table: "WorkspacesMembers");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspacesMembers_UserId_WorkspaceId",
                table: "WorkspacesMembers",
                columns: new[] { "UserId", "WorkspaceId" });
        }
    }
}
