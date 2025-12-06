using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Msd.Services.AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserAminAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p", "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p", "Admin", "ADMIN" },
                    { "3c4d5e6f-7g8h-9i0j-1k2l-3m4n5o6p7q8r", "3c4d5e6f-7g8h-9i0j-1k2l-3m4n5o6p7q8r", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q", 0, "57a12d54-ffab-4d7d-b9af-63aafb5ff343", "admin@admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAEON7JJZ1m+JZHEBg6p48P+e1qe0DahcLPI73SaIlKDaBIxb7ehoAiA3TbCOzun7SgQ==", null, false, "7749f13b-857d-40fd-9d00-96e56a5a1dc2", false, "admin@admin.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p", "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c4d5e6f-7g8h-9i0j-1k2l-3m4n5o6p7q8r");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p", "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q");
        }
    }
}

