using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using online_mr_certi.Data;

#nullable disable

namespace online_mr_certi.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260507153000_AddAdminNotificationReadState")]
public partial class AddAdminNotificationReadState : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS `admin_notification_read_states` (
              `UserId` int NOT NULL,
              `LastReadAt` datetime(6) NOT NULL,
              PRIMARY KEY (`UserId`),
              CONSTRAINT `FK_admin_notification_read_states_users_UserId`
                FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
            ) CHARACTER SET=utf8mb4;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DROP TABLE IF EXISTS `admin_notification_read_states`;
            """);
    }
}

