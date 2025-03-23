using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayReservationWeb.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Reservations_ReservationID",
                table: "Passengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Users_UserID",
                table: "Passengers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ReservationID",
                table: "Passengers",
                newName: "PNRNo");

            migrationBuilder.RenameIndex(
                name: "IX_Passengers_ReservationID",
                table: "Passengers",
                newName: "IX_Passengers_PNRNo");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Passengers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Reservations_PNRNo",
                table: "Passengers",
                column: "PNRNo",
                principalTable: "Reservations",
                principalColumn: "PNRNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Users_UserID",
                table: "Passengers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Reservations_PNRNo",
                table: "Passengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Users_UserID",
                table: "Passengers");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PNRNo",
                table: "Passengers",
                newName: "ReservationID");

            migrationBuilder.RenameIndex(
                name: "IX_Passengers_PNRNo",
                table: "Passengers",
                newName: "IX_Passengers_ReservationID");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Passengers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Reservations_ReservationID",
                table: "Passengers",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "PNRNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Users_UserID",
                table: "Passengers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
