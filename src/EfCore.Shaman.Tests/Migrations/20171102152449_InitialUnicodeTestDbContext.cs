using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using EfCore.Shaman.Tests.Model;

namespace EfCore.Shaman.Tests.Migrations
{
    public partial class InitialUnicodeTestDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MySettingsEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MySettingsEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SomeEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Default = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DefaultNoLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoUnicode = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                    NoUnicodeNoLength = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Unicode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UnicodeNoLength = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SomeEntity", x => x.Id);
                });
            migrationBuilder.FixMigrationUp<UnicodeTestDbContext>();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MySettingsEntity");

            migrationBuilder.DropTable(
                name: "SomeEntity");
        }
    }
}
