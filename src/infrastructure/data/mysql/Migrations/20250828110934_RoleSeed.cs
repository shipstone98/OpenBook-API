using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

using Shipstone.OpenBook.Api.Core.Accounts;

#nullable disable

namespace Shipstone.OpenBook.Api.Infrastructure.Data.MySql.Migrations
{
    /// <inheritdoc />
    public partial class RoleSeed : Migration
    {
        private readonly IEnumerable<(long Id, String Name)> _roles;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleSeed" /> class.
        /// </summary>
        public RoleSeed() =>
            this._roles = new (long, String)[]
            {
                (Roles.AdministratorId, Roles.Administrator),
                (Roles.SystemAdministratorId, Roles.SystemAdministrator),
                (Roles.UserId, Roles.User),
            };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            String[] columns =
                new[] { "Id", "Created", "Updated", "Name", "NameNormalized" };

            DateTime now = DateTime.UtcNow;

            foreach ((long id, String name) in this._roles)
            {
                String nameNormalized = name.ToUpperInvariant();

                Object[] values =
                    new Object[] { id, now, now, name, nameNormalized };

                migrationBuilder.InsertData("Roles", columns, values);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach ((long id, _) in this._roles)
            {
                migrationBuilder.DeleteData("Roles", "Id", id);
            }
        }
    }
}
