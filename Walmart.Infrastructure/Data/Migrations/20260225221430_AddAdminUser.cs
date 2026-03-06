using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Walmart.Data.Migrations
{
    
    public partial class AddAdminUser : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO[security].[Users]([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [LastName], [ProfilePicture], [FirstName]) VALUES(N'6db86cf4-5c79-46e0-978b-8be0831682cc', N'admin', N'ADMIN', N'admin@gmail.com', N'ADMIN@GMAIL.COM', 0, N'AQAAAAIAAYagAAAAEAMJsJoOFFgNk6vgzll+NhaU3er/4JLgiy6L+8i5dgZHFkC9/fQcQohwJrf43qWbbQ==', N'C7ZISYEOE5CVOPFQWAKM7ENPJOABMD7T', N'dede9ba9-d0be-4386-8a53-a9d852528d1f', NULL, 0, 0, NULL, 1, 0, N'Alsayed', NULL, N'Ahmed')"
);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[Users] WHERE Id = '6db86cf4-5c79-46e0-978b-8be0831682cc'");
        }
    }
}
