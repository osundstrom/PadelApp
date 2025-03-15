using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PadelApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class padel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PadelCourts",
                columns: table => new
                {
                    CourtId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CourtName = table.Column<string>(type: "TEXT", nullable: false),
                    CourtType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PadelCourts", x => x.CourtId);
                });

            migrationBuilder.CreateTable(
                name: "PadelBookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookingTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CourtId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PadelBookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_PadelBookings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PadelBookings_PadelCourts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "PadelCourts",
                        principalColumn: "CourtId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PadelBookings_CourtId",
                table: "PadelBookings",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_PadelBookings_UserId",
                table: "PadelBookings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PadelBookings");

            migrationBuilder.DropTable(
                name: "PadelCourts");
        }
    }
}
