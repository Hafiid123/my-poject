using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using online_mr_certi.Data;

#nullable disable

namespace online_mr_certi.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260507141500_AddApplicationDecisionDate")]
public partial class AddApplicationDecisionDate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE `marriage_applications`
            ADD COLUMN `DecisionDate` datetime(6) NULL;

            UPDATE `marriage_applications`
            SET `DecisionDate` = `SubmissionDate`
            WHERE `DecisionDate` IS NULL
              AND (`Status` = 'Approved' OR `Status` = 'Rejected');
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE `marriage_applications`
            DROP COLUMN `DecisionDate`;
            """);
    }
}

