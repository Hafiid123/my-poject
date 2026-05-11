using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using online_mr_certi.Data;

#nullable disable

namespace online_mr_certi.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260507103000_AddRbacCore")]
public partial class AddRbacCore : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS `roles` (
              `Id` int NOT NULL AUTO_INCREMENT,
              `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
              `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
              PRIMARY KEY (`Id`),
              UNIQUE KEY `UX_roles_Name` (`Name`)
            ) CHARACTER SET=utf8mb4;

            CREATE TABLE IF NOT EXISTS `permissions` (
              `Id` int NOT NULL AUTO_INCREMENT,
              `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
              PRIMARY KEY (`Id`),
              UNIQUE KEY `UX_permissions_Name` (`Name`)
            ) CHARACTER SET=utf8mb4;

            CREATE TABLE IF NOT EXISTS `role_permissions` (
              `RoleId` int NOT NULL,
              `PermissionId` int NOT NULL,
              PRIMARY KEY (`RoleId`, `PermissionId`),
              CONSTRAINT `FK_role_permissions_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE,
              CONSTRAINT `FK_role_permissions_permissions_PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE CASCADE
            ) CHARACTER SET=utf8mb4;

            CREATE INDEX `IX_role_permissions_PermissionId` ON `role_permissions` (`PermissionId`);
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `users` ADD COLUMN `RoleId` int NULL;
            """);

        migrationBuilder.Sql(
            """
            CREATE INDEX `IX_users_RoleId` ON `users` (`RoleId`);
            ALTER TABLE `users`
              ADD CONSTRAINT `FK_users_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE RESTRICT;
            """);

        // Seed default roles (Admin + User) and base permission list.
        migrationBuilder.Sql(
            """
            INSERT INTO `roles` (`Name`) VALUES ('User') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `roles` (`Name`) VALUES ('Admin') ON DUPLICATE KEY UPDATE `Name` = `Name`;

            INSERT INTO `permissions` (`Name`) VALUES ('CreateApplication') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('ViewApplication') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('ApproveApplication') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('IssueCertificate') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('ManageUsers') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('ManageRoles') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('ManageFees') ON DUPLICATE KEY UPDATE `Name` = `Name`;
            INSERT INTO `permissions` (`Name`) VALUES ('ManagePayments') ON DUPLICATE KEY UPDATE `Name` = `Name`;

            -- Give Admin all permissions
            INSERT IGNORE INTO `role_permissions` (`RoleId`, `PermissionId`)
            SELECT r.Id, p.Id
            FROM `roles` r
            JOIN `permissions` p
            WHERE r.Name = 'Admin';

            -- Map existing users: Admin string-role -> Admin roleId, everyone else -> User roleId (including portal registrations)
            UPDATE `users` u
            JOIN `roles` r_admin ON r_admin.Name = 'Admin'
            JOIN `roles` r_user ON r_user.Name = 'User'
            SET u.RoleId = CASE WHEN u.Role = 'Admin' THEN r_admin.Id ELSE r_user.Id END
            WHERE u.RoleId IS NULL;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE `users` DROP FOREIGN KEY `FK_users_roles_RoleId`;
            DROP INDEX `IX_users_RoleId` ON `users`;
            ALTER TABLE `users` DROP COLUMN `RoleId`;

            DROP TABLE IF EXISTS `role_permissions`;
            DROP TABLE IF EXISTS `permissions`;
            DROP TABLE IF EXISTS `roles`;
            """);
    }
}

