using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TIE_Decor.Migrations
{
    /// <inheritdoc />
    public partial class updateconsultation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_Users_UserId1",
                table: "Consultations");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_UserId1",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Consultations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Consultations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_UserId",
                table: "Consultations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_Users_UserId",
                table: "Consultations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_Users_UserId",
                table: "Consultations");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_UserId",
                table: "Consultations");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Consultations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Consultations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_UserId1",
                table: "Consultations",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_Users_UserId1",
                table: "Consultations",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
