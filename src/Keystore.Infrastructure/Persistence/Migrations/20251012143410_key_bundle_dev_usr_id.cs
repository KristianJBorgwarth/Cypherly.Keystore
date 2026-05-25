using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keystore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class key_bundle_dev_usr_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "key_bundle",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_id",
                table: "key_bundle");
        }
    }
}
