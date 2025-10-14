using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipstone.OpenBook.Api.Infrastructure.Data.MySql.Migrations
{
    /// <inheritdoc />
    public partial class UserBorn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Born",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Born",
                table: "Users");
        }
    }
}
