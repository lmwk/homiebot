using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.DAL.Migrations.Migrations
{
    public partial class Guild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guildid = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    muteroleid = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    novcroleid = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    usercount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
