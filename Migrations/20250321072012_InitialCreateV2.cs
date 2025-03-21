using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayReservationWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trains",
                columns: table => new
                {
                    TrainID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fare = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SeatsAvailable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.TrainID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quota",
                columns: table => new
                {
                    QuotaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainID = table.Column<int>(type: "int", nullable: false),
                    QuotaType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatsAvailable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quota", x => x.QuotaID);
                    table.ForeignKey(
                        name: "FK_Quota_Trains_TrainID",
                        column: x => x.TrainID,
                        principalTable: "Trains",
                        principalColumn: "TrainID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    PNRNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainID = table.Column<int>(type: "int", nullable: false),
                    QuotaID = table.Column<int>(type: "int", nullable: false),
                    JourneyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClassType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalFare = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CancellationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatsBooked = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.PNRNo);
                    table.ForeignKey(
                        name: "FK_Reservations_Quota_QuotaID",
                        column: x => x.QuotaID,
                        principalTable: "Quota",
                        principalColumn: "QuotaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Trains_TrainID",
                        column: x => x.TrainID,
                        principalTable: "Trains",
                        principalColumn: "TrainID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    PassengerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    PassengerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReservationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.PassengerID);
                    table.ForeignKey(
                        name: "FK_Passengers_Reservations_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "Reservations",
                        principalColumn: "PNRNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Passengers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_ReservationID",
                table: "Passengers",
                column: "ReservationID");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_UserID",
                table: "Passengers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Quota_TrainID",
                table: "Quota",
                column: "TrainID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_QuotaID",
                table: "Reservations",
                column: "QuotaID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TrainID",
                table: "Reservations",
                column: "TrainID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Quota");

            migrationBuilder.DropTable(
                name: "Trains");
        }
    }
}
