using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keystore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class kyber_last_resort_key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "kyber_pre_key_id",
                table: "key_bundle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "kyber_pre_key_public",
                table: "key_bundle",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "kyber_pre_key_signature",
                table: "key_bundle",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kyber_pre_key_id",
                table: "key_bundle");

            migrationBuilder.DropColumn(
                name: "kyber_pre_key_public",
                table: "key_bundle");

            migrationBuilder.DropColumn(
                name: "kyber_pre_key_signature",
                table: "key_bundle");
        }
    }
}
