using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace online_mr_certi.Data.Migrations;

/// <inheritdoc />
public partial class ManualPaymentGatewayFlow : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE `users` ADD `PaymentStatus` varchar(30) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Unpaid';
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `fees` ADD `Currency` varchar(10) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'USD';
            ALTER TABLE `fees` ADD `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6);
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `payments` ADD `UserId` int NULL;
            ALTER TABLE `payments` ADD `FeeId` int NULL;
            ALTER TABLE `payments` ADD `SenderPhone` varchar(50) CHARACTER SET utf8mb4 NULL;
            ALTER TABLE `payments` ADD `TransactionNumber` varchar(100) CHARACTER SET utf8mb4 NULL;
            ALTER TABLE `payments` ADD `CreatedAt` datetime(6) NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE `payments` p
            INNER JOIN `marriage_applications` m ON m.Id = p.ApplicationId
            SET p.UserId = m.UserId
            WHERE p.UserId IS NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE `payments` p
            SET p.FeeId = (SELECT f.Id FROM `fees` f ORDER BY f.Id ASC LIMIT 1)
            WHERE p.FeeId IS NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE `payments`
            SET `CreatedAt` = COALESCE(`PaymentDate`, UTC_TIMESTAMP(6))
            WHERE `CreatedAt` IS NULL;
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `payments` MODIFY `UserId` int NOT NULL;
            ALTER TABLE `payments` MODIFY `FeeId` int NOT NULL;
            ALTER TABLE `payments` MODIFY `CreatedAt` datetime(6) NOT NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE `payments` SET `PaymentStatus` = 'Approved' WHERE `PaymentStatus` = 'Paid';
            UPDATE `payments` SET `PaymentStatus` = 'Rejected' WHERE `PaymentStatus` = 'Failed';
            """);

        migrationBuilder.CreateIndex(
            name: "IX_payments_UserId",
            table: "payments",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_payments_FeeId",
            table: "payments",
            column: "FeeId");

        migrationBuilder.AddForeignKey(
            name: "FK_payments_users_UserId",
            table: "payments",
            column: "UserId",
            principalTable: "users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_payments_fees_FeeId",
            table: "payments",
            column: "FeeId",
            principalTable: "fees",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_payments_fees_FeeId",
            table: "payments");

        migrationBuilder.DropForeignKey(
            name: "FK_payments_users_UserId",
            table: "payments");

        migrationBuilder.DropIndex(
            name: "IX_payments_FeeId",
            table: "payments");

        migrationBuilder.DropIndex(
            name: "IX_payments_UserId",
            table: "payments");

        migrationBuilder.Sql(
            """
            UPDATE `payments` SET `PaymentStatus` = 'Paid' WHERE `PaymentStatus` = 'Approved';
            UPDATE `payments` SET `PaymentStatus` = 'Failed' WHERE `PaymentStatus` = 'Rejected';
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `payments` DROP COLUMN `CreatedAt`;
            ALTER TABLE `payments` DROP COLUMN `TransactionNumber`;
            ALTER TABLE `payments` DROP COLUMN `SenderPhone`;
            ALTER TABLE `payments` DROP COLUMN `FeeId`;
            ALTER TABLE `payments` DROP COLUMN `UserId`;
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `fees` DROP COLUMN `CreatedAt`;
            ALTER TABLE `fees` DROP COLUMN `Currency`;
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `users` DROP COLUMN `PaymentStatus`;
            """);
    }
}
