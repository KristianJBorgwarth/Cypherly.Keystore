﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.Keystore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class outbox_table_rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                newName: "outbox_message");

            migrationBuilder.AddPrimaryKey(
                name: "PK_outbox_message",
                table: "outbox_message",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_outbox_message",
                table: "outbox_message");

            migrationBuilder.RenameTable(
                name: "outbox_message",
                newName: "OutboxMessage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage",
                column: "Id");
        }
    }
}
