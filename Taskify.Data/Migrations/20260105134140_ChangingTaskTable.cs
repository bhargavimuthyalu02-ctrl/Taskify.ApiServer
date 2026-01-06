using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskify.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangingTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerEmail",
                table: "Tasks",
                newName: "UserEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Tasks",
                newName: "OwnerEmail");
        }
    }
}
