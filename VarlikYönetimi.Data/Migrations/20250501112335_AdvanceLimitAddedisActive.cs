using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VarlikYönetimi.Data.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdvanceLimitAddedisActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AdvanceLimits",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AdvanceLimits");
        }
    }
}
