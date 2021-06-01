using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.DAL.Migrations.Migrations
{
    public partial class UpdateGuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "botbanroleid",
                table: "Guilds",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "botbanroleid",
                table: "Guilds");
        }
    }
}
