using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EMRedemption.Data.Migrations
{
    public partial class regeneratemodel : Migration
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

            migrationBuilder.CreateTable(
                name: "Redemptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    FetchBy = table.Column<string>(maxLength: 30, nullable: true),
                    FetchDateTime = table.Column<DateTime>(nullable: false),
                    RedeemDateTime = table.Column<DateTime>(nullable: false),
                    RetailerEmailAddress = table.Column<string>(maxLength: 100, nullable: true),
                    RetailerName = table.Column<string>(maxLength: 100, nullable: true),
                    RetailerPhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    RetailerStoreName = table.Column<string>(maxLength: 100, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TransactionID = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redemptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RewardTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Code = table.Column<string>(maxLength: 5, nullable: true),
                    RewardName = table.Column<string>(maxLength: 100, nullable: true),
                    RewardTypeName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedemptionItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Points = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    RedemptionId = table.Column<int>(nullable: false),
                    RewardCode = table.Column<string>(maxLength: 5, nullable: true),
                    RewardName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedemptionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedemptionItems_Redemptions_RedemptionId",
                        column: x => x.RedemptionId,
                        principalTable: "Redemptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AddBy = table.Column<string>(maxLength: 30, nullable: true),
                    AddDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    LotNo = table.Column<string>(maxLength: 10, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    RedemptionItemId = table.Column<int>(nullable: true),
                    RewardCode = table.Column<string>(maxLength: 5, nullable: true),
                    RewardName = table.Column<string>(maxLength: 100, nullable: true),
                    RewardTypeId = table.Column<int>(nullable: false),
                    RewardTypeName = table.Column<string>(maxLength: 100, nullable: true),
                    SerialNo = table.Column<string>(maxLength: 50, nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rewards_RedemptionItems_RedemptionItemId",
                        column: x => x.RedemptionItemId,
                        principalTable: "RedemptionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rewards_RewardTypes_RewardTypeId",
                        column: x => x.RewardTypeId,
                        principalTable: "RewardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RedemptionItems_RedemptionId",
                table: "RedemptionItems",
                column: "RedemptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_RedemptionItemId",
                table: "Rewards",
                column: "RedemptionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_RewardTypeId",
                table: "Rewards",
                column: "RewardTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLogs");

            migrationBuilder.DropTable(
                name: "Rewards");

            migrationBuilder.DropTable(
                name: "RedemptionItems");

            migrationBuilder.DropTable(
                name: "RewardTypes");

            migrationBuilder.DropTable(
                name: "Redemptions");
        }
    }
}
