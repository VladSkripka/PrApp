using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "02174cf0-9412-4cfe-afbf-59f706d72cf6", "02174cf0-9412-4cfe-afbf-59f706d72cf6", "Admin", "ADMIN" },
                    { "6DFA1936-12B4-41AF-BF73-30A62203F000", "6DFA1936-12B4-41AF-BF73-30A62203F000", "Provider", "PROVIDER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "341743f0-asd2-42de-afbf-59kmkkmk72cf6", 0, "871d9f8c-06c3-4be7-b33e-5d90c0ed5121", "admin@gmail.com", true, false, null, null, "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEJNUVnyyV5lgxCJ4UJsaz2LoG1/b3NFe6ml0UAt3h4NYM2J0Exa+7yPzx7HcDFTefQ==", null, false, "ee4eb003-1bd3-4c0c-9a32-143d3b6f950b", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "02174cf0-9412-4cfe-afbf-59f706d72cf6", "341743f0-asd2-42de-afbf-59kmkkmk72cf6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6DFA1936-12B4-41AF-BF73-30A62203F000");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "02174cf0-9412-4cfe-afbf-59f706d72cf6", "341743f0-asd2-42de-afbf-59kmkkmk72cf6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02174cf0-9412-4cfe-afbf-59f706d72cf6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2-42de-afbf-59kmkkmk72cf6");
        }
    }
}
