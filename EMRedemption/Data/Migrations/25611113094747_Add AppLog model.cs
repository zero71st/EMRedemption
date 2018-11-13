using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EMRedemption.Data.Migrations
{
    public partial class AddAppLogmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Application = table.Column<string>(maxLength: 50, nullable: true),
                    Callsite = table.Column<string>(maxLength: 512, nullable: true),
                    Exception = table.Column<string>(maxLength: 512, nullable: true),
                    Level = table.Column<string>(maxLength: 50, nullable: true),
                    Logged = table.Column<DateTime>(nullable: false),
                    Logger = table.Column<string>(maxLength: 250, nullable: true),
                    Message = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLogs");
        }
    }
}
