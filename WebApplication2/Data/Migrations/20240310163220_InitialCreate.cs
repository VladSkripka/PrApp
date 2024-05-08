using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trains",
                columns: table => new
                {
                    trainTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    typeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trainSeats = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.trainTypeID);
                });

            migrationBuilder.CreateTable(
                name: "VacantDepartures",
                columns: table => new
                {
                    vdID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    vacanDepartureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departureP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departureCoord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    arrivalP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departureD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    arrivalD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    seatsCount = table.Column<int>(type: "int", nullable: false),
                    trainTypeID = table.Column<int>(type: "int", nullable: false),
                    providerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacantDepartures", x => x.vdID);
                    table.ForeignKey(
                        name: "FK_VacantDepartures_AspNetUsers_providerId",
                        column: x => x.providerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacantDepartures_Trains_trainTypeID",
                        column: x => x.trainTypeID,
                        principalTable: "Trains",
                        principalColumn: "trainTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    ticketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ticketPID = table.Column<int>(type: "int", nullable: false),
                    departureP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    arrivalP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departureD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    arrivalD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    carriageNumber = table.Column<int>(type: "int", nullable: false),
                    seat = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vacDepvdID = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.ticketID);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tickets_VacantDepartures_vacDepvdID",
                        column: x => x.vacDepvdID,
                        principalTable: "VacantDepartures",
                        principalColumn: "vdID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_userId",
                table: "Tickets",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_vacDepvdID",
                table: "Tickets",
                column: "vacDepvdID");

            migrationBuilder.CreateIndex(
                name: "IX_VacantDepartures_providerId",
                table: "VacantDepartures",
                column: "providerId");

            migrationBuilder.CreateIndex(
                name: "IX_VacantDepartures_trainTypeID",
                table: "VacantDepartures",
                column: "trainTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "VacantDepartures");

            migrationBuilder.DropTable(
                name: "Trains");
        }
    }
}
