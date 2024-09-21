using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TIE_Decor.Migrations
{
    /// <inheritdoc />
    public partial class Adddatacatebrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert data into Brands table (let the IDENTITY column handle Id generation)
            migrationBuilder.Sql("INSERT INTO [dbo].[Brands] ([Name]) VALUES ('IKEA')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Brands] ([Name]) VALUES ('H&M Home')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Brands] ([Name]) VALUES ('Zara Home')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Brands] ([Name]) VALUES ('Cassina')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Brands] ([Name]) VALUES ('Ralph Lauren')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Brands] ([Name]) VALUES ('Versace')");

            // Insert data into Categories table (let the IDENTITY column handle CategoryId generation)
            migrationBuilder.Sql("INSERT INTO [dbo].[Categories] ([CategoryName]) VALUES ('Furniture')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Categories] ([CategoryName]) VALUES ('Lighting')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Categories] ([CategoryName]) VALUES ('Decor')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Categories] ([CategoryName]) VALUES ('Rugs and Carpets')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Categories] ([CategoryName]) VALUES ('Wall Art')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Categories] ([CategoryName]) VALUES ('Curtains and Blinds')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Optional: If you want to delete the inserted data during rollback
            migrationBuilder.Sql("DELETE FROM [dbo].[Brands] WHERE [Name] IN ('IKEA', 'H&M Home', 'Zara Home', 'Cassina', 'Ralph Lauren', 'Versace')");
            migrationBuilder.Sql("DELETE FROM [dbo].[Categories] WHERE [CategoryName] IN ('Furniture', 'Lighting', 'Decor', 'Rugs and Carpets', 'Wall Art', 'Curtains and Blinds')");
        }

    }
}
