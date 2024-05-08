using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    /// <inheritdoc />
    public partial class closForAgeOfTheCustAndAdditionalOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "addtionalOptions",
                table: "TicketModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ticketAgeType",
                table: "TicketModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "63710cc3-a402-4ec2-bafe-e7e0e382ce51", "AQAAAAIAAYagAAAAEBl7WPXbuR3O/zNfMY8uAYmBUocHHQShNuIsnV+EPZmzKRQFcCx28PdC1xoLqPy2lg==", "ce6f1af4-e379-4703-b445-b13c1b392160" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "addtionalOptions",
                table: "TicketModels");

            migrationBuilder.DropColumn(
                name: "ticketAgeType",
                table: "TicketModels");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "99decbdc-4267-4354-b769-97f284c09cf4", "AQAAAAIAAYagAAAAEHqyZvS9w6Z/Q7LRgPNcYHk+683S1uCLndDdMi80HzskrOI29MiWYhhH3c9L9UT3xw==", "8882477d-34ce-4412-b930-32993189fb74" });
        }
    }
}
