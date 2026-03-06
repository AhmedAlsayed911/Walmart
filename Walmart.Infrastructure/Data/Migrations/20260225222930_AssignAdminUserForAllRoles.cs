using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Walmart.Data.Migrations
{
    
    public partial class AssignAdminUserForAllRoles : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [security].[UserRoles] (UserId , RoleId) SELECT '6db86cf4-5c79-46e0-978b-8be0831682cc', Id FROM [security].[Roles]");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[UserRoles] WHERE UserId = '6db86cf4-5c79-46e0-978b-8be0831682cc'");
        }
    }
}
