using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    /// <inheritdoc />
    public partial class newColsForTrainsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "schemaPath",
                table: "Trains",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "trainRows",
                table: "Trains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "99decbdc-4267-4354-b769-97f284c09cf4", "AQAAAAIAAYagAAAAEHqyZvS9w6Z/Q7LRgPNcYHk+683S1uCLndDdMi80HzskrOI29MiWYhhH3c9L9UT3xw==", "8882477d-34ce-4412-b930-32993189fb74" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "schemaPath",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "trainRows",
                table: "Trains");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "58f8a356-afdf-49c8-b82c-f28fc0cd013a", "AQAAAAIAAYagAAAAEMnWqBeUcEuGG1Tojkzd2RIfR1Eqdk1tL4TBW2UQp3/Yiu6xFw5eRRxijAKAqayrUA==", "8d3b453f-6b27-4c44-b8bb-034526a59dc3" });
        }
    }
}
