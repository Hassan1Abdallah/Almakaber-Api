using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Almakaber.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRemindersToggle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribedToReminders",
                table: "DeceasedSupplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribedToReminders",
                table: "DeceasedSupplications");
        }
    }
}
