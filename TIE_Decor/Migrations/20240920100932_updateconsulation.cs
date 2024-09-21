using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TIE_Decor.Migrations
{
    /// <inheritdoc />
    public partial class updateconsulation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DesignerId",
                table: "Consultations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesignerId",
                table: "Consultations");
        }
    }
}
