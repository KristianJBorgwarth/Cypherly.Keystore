using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keystore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class remove_consumed_prekeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM pre_key WHERE consumed = true;");

            migrationBuilder.DropIndex(
                name: "IX_pre_key_key_bundle_id",
                table: "pre_key");

            migrationBuilder.DropColumn(
                name: "consumed",
                table: "pre_key");

            migrationBuilder.CreateIndex(
                name: "idx_key_bundle_key_id",
                table: "pre_key",
                columns: new[] { "key_bundle_id", "key_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_key_bundle_key_id",
                table: "pre_key");

            migrationBuilder.AddColumn<bool>(
                name: "consumed",
                table: "pre_key",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_pre_key_key_bundle_id",
                table: "pre_key",
                column: "key_bundle_id");
        }
    }
}
