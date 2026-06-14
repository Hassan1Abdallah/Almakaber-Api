using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Almakaber.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRemindersTogglev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSubscribedToReminders",
                table: "DeceasedSupplications",
                newName: "WantsAnnualReminder");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "WantsAnnualReminder",
                table: "DeceasedSupplications",
                newName: "IsSubscribedToReminders");
        }
    }
}
