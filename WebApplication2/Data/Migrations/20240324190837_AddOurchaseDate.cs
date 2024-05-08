using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOurchaseDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "TicketModels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "314f1fdc-242e-49ca-aca1-0f20c3de100d", "AQAAAAIAAYagAAAAEE3SivaYknzwcAmSjlUWaICATLW8Zdqb1urn7z35vWV0XKu1KHhzM2Wva3/KQHxP+Q==", "5ec58722-ae79-4512-878f-d77766789d78" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "TicketModels");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "871d9f8c-06c3-4be7-b33e-5d90c0ed5121", "AQAAAAIAAYagAAAAEJNUVnyyV5lgxCJ4UJsaz2LoG1/b3NFe6ml0UAt3h4NYM2J0Exa+7yPzx7HcDFTefQ==", "ee4eb003-1bd3-4c0c-9a32-143d3b6f950b" });
        }
    }
}
