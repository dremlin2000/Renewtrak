using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Glossary.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Glossary",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Term = table.Column<string>(nullable: true),
                    Definition = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Glossary", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Glossary",
                columns: new[] { "Id", "Definition", "Term" },
                values: new object[] { new Guid("705b3587-eec0-434e-90ae-7e0a3d3326e3"), "The ocean floor offshore from the continental margin, usually very flat with a slight slope.", "abyssal plain" });

            migrationBuilder.InsertData(
                table: "Glossary",
                columns: new[] { "Id", "Definition", "Term" },
                values: new object[] { new Guid("ebffa4ff-727c-4736-a316-0ae006b1c8c7"), "To add terranes (small land masses or pieces of crust) to another, usually larger, land mass.", "accrete" });

            migrationBuilder.InsertData(
                table: "Glossary",
                columns: new[] { "Id", "Definition", "Term" },
                values: new object[] { new Guid("f623e3a8-3858-4336-a5c5-bb397013b75d"), "Term pertaining to a highly basic, as opposed to acidic, subtance. For example, hydroxide or carbonate of sodium or potassium.", "alkaline" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Glossary");
        }
    }
}
