using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keystore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "key_bundle",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_key = table.Column<Guid>(type: "uuid", nullable: false),
                    identity_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    registration_id = table.Column<int>(type: "integer", nullable: false),
                    signed_pre_key_id = table.Column<int>(type: "integer", nullable: false),
                    signed_pre_key_public = table.Column<byte[]>(type: "bytea", nullable: false),
                    signed_pre_key_signature = table.Column<byte[]>(type: "bytea", nullable: false),
                    signed_pre_key_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_key_bundle", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    occurred_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_message", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pre_key",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key_bundle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    key_id = table.Column<int>(type: "integer", nullable: false),
                    public_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    consumed = table.Column<bool>(type: "boolean", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pre_key", x => x.id);
                    table.ForeignKey(
                        name: "FK_pre_key_key_bundle_key_bundle_id",
                        column: x => x.key_bundle_id,
                        principalTable: "key_bundle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_access_key",
                table: "key_bundle",
                column: "access_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pre_key_key_bundle_id",
                table: "pre_key",
                column: "key_bundle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_message");

            migrationBuilder.DropTable(
                name: "pre_key");

            migrationBuilder.DropTable(
                name: "key_bundle");
        }
    }
}
