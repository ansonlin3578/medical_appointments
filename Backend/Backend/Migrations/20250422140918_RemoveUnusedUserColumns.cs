using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedUserColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key for UserId1
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_UserId1",
                table: "Appointments");

            // Drop index for UserId1
            migrationBuilder.DropIndex(
                name: "IX_Appointments_UserId1",
                table: "Appointments");

            // Drop only the UserId1 column
            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add UserId1 column back
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Appointments",
                type: "integer",
                nullable: true);

            // Create index for UserId1
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId1",
                table: "Appointments",
                column: "UserId1");

            // Add foreign key back
            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_UserId1",
                table: "Appointments",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
