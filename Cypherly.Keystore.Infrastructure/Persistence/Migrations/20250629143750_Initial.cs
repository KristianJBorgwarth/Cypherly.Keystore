using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.Keystore.Infrastructure.Persistence.Migrations
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentityKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    RegistrationId = table.Column<int>(type: "integer", nullable: false),
                    SignedPrekeyId = table.Column<int>(type: "integer", nullable: false),
                    SignedPreKeyPublic = table.Column<byte[]>(type: "bytea", nullable: false),
                    SignedPreKeySignature = table.Column<byte[]>(type: "bytea", nullable: false),
                    SignedPreKeyTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_key_bundle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pre_key",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyBundleId = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyId = table.Column<int>(type: "integer", nullable: false),
                    PublicKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    Consumed = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pre_key", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pre_key_key_bundle_KeyBundleId",
                        column: x => x.KeyBundleId,
                        principalTable: "key_bundle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_key_bundle_AccessKey",
                table: "key_bundle",
                column: "AccessKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pre_key_KeyBundleId",
                table: "pre_key",
                column: "KeyBundleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "pre_key");

            migrationBuilder.DropTable(
                name: "key_bundle");
        }
    }
}
