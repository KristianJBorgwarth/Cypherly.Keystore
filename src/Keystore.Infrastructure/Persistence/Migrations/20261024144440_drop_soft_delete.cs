using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keystore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class drop_soft_delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted",
                table: "pre_key");

            migrationBuilder.DropColumn(
                name: "deleted",
                table: "key_bundle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted",
                table: "pre_key",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted",
                table: "key_bundle",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
