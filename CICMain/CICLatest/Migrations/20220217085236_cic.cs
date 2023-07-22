using Microsoft.EntityFrameworkCore.Migrations;

namespace CICLatest.Migrations
{
    public partial class cic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustNo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

          

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "CustNo",
                table: "AspNetUsers");
        }
    }
}
