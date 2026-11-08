using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunnelWatch.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMachineData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gateways",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_message_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gateways", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "latest_signal_values",
                columns: table => new
                {
                    gateway_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    signal_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_latest_signal_values", x => new { x.gateway_id, x.signal_name });
                    table.ForeignKey(
                        name: "FK_latest_signal_values_gateways_gateway_id",
                        column: x => x.gateway_id,
                        principalTable: "gateways",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "signal_history",
                columns: table => new
                {
                    gateway_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    signal_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    minute = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    minimum = table.Column<double>(type: "double precision", nullable: false),
                    maximum = table.Column<double>(type: "double precision", nullable: false),
                    average = table.Column<double>(type: "double precision", nullable: false),
                    sample_count = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signal_history", x => new { x.gateway_id, x.signal_name, x.minute });
                    table.ForeignKey(
                        name: "FK_signal_history_gateways_gateway_id",
                        column: x => x.gateway_id,
                        principalTable: "gateways",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "latest_signal_values");

            migrationBuilder.DropTable(
                name: "signal_history");

            migrationBuilder.DropTable(
                name: "gateways");
        }
    }
}
