using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueWASIdnty.Server.Data.Migrations
{
    public partial class UserUPdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResgistrationNo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResgistrationNo",
                table: "AspNetUsers");
        }
    }
}
