using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OSAutoRepair.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopdomainModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClosedDates",
                columns: table => new
                {
                    ClosedDateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedDates", x => x.ClosedDateId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    ServiceCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultDurationHours = table.Column<int>(type: "int", nullable: false),
                    BaseEstimate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.ServiceCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "TireStorageItems",
                columns: table => new
                {
                    TireStorageItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerPhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TireDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TireStorageItems", x => x.TireStorageItemId);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentAppointmentId = table.Column<int>(type: "int", nullable: true),
                    BayNumber = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartHour = table.Column<int>(type: "int", nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    ServiceCategoryId = table.Column<int>(type: "int", nullable: false),
                    CustomerFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerServiceNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TechnicianServiceNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_Appointments_ParentAppointmentId",
                        column: x => x.ParentAppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId");
                    table.ForeignKey(
                        name: "FK_Appointments_ServiceCategories_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "ServiceCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ServiceCategories",
                columns: new[] { "ServiceCategoryId", "BaseEstimate", "DefaultDurationHours", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 0.00m, 1, true, "Oil" },
                    { 2, 0.00m, 1, true, "Repair" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ParentAppointmentId",
                table: "Appointments",
                column: "ParentAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceCategoryId",
                table: "Appointments",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedDates_Date",
                table: "ClosedDates",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "ClosedDates");

            migrationBuilder.DropTable(
                name: "TireStorageItems");

            migrationBuilder.DropTable(
                name: "ServiceCategories");
        }
    }
}
