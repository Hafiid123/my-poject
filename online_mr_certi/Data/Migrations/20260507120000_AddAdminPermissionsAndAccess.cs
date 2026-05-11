using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using online_mr_certi.Data;

#nullable disable

namespace online_mr_certi.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260507120000_AddAdminPermissionsAndAccess")]
public partial class AddAdminPermissionsAndAccess : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            INSERT INTO `permissions` (`Name`) VALUES ('ViewDashboard') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('ApproveApplications') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('RejectApplications') ON DUPLICATE KEY UPDATE `Name` = `Name`;

            -- Ensure Admin keeps all permissions (including new ones)
            INSERT IGNORE INTO `role_permissions` (`RoleId`, `PermissionId`)
            SELECT r.Id, p.Id
            FROM `roles` r
            JOIN `permissions` p
            WHERE r.Name = 'Admin';
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Leave permissions in place (safer, avoids breaking role mappings)
    }
}

