using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Craft.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Craft_Locales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CultureCode = table.Column<string>(
                        type: "text",
                        nullable: false
                    ),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Craft_Locales", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(
                        type: "text",
                        nullable: false
                    ),
                    IsCompleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Craft_Locales");

            migrationBuilder.DropTable(name: "Todos");
        }
    }
}
