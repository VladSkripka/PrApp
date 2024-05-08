using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    /// <inheritdoc />
    public partial class initCeate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketModels",
                columns: table => new
                {
                    ticketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ticketPID = table.Column<int>(type: "int", nullable: false),
                    departureP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    arrivalP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departureD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    arrivalD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    passengerFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    carriageNumber = table.Column<int>(type: "int", nullable: false),
                    seat = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vacantDepatureID = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketModels", x => x.ticketID);
                    table.ForeignKey(
                        name: "FK_TicketModels_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketModels_userId",
                table: "TicketModels",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketModels");
        }
    }
}
