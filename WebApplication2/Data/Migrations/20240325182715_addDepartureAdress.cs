using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDepartureAdress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "departureAdress",
                table: "VacantDepartures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "58f8a356-afdf-49c8-b82c-f28fc0cd013a", "AQAAAAIAAYagAAAAEMnWqBeUcEuGG1Tojkzd2RIfR1Eqdk1tL4TBW2UQp3/Yiu6xFw5eRRxijAKAqayrUA==", "8d3b453f-6b27-4c44-b8bb-034526a59dc3" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "departureAdress",
                table: "VacantDepartures");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "314f1fdc-242e-49ca-aca1-0f20c3de100d", "AQAAAAIAAYagAAAAEE3SivaYknzwcAmSjlUWaICATLW8Zdqb1urn7z35vWV0XKu1KHhzM2Wva3/KQHxP+Q==", "5ec58722-ae79-4512-878f-d77766789d78" });
        }
    }
}
