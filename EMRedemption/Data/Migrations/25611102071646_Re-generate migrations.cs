using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EMRedemption.Data.Migrations
{
    public partial class Regeneratemigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Redemptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    FetchDateTime = table.Column<DateTime>(nullable: false),
                    RedeemDateTime = table.Column<DateTime>(nullable: false),
                    RetailerEmailAddress = table.Column<string>(nullable: true),
                    RetailerName = table.Column<string>(nullable: true),
                    RetailerPhoneNumber = table.Column<string>(nullable: true),
                    RetailerStoreName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TransactionID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redemptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedemptionItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Points = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    RedemptionId = table.Column<int>(nullable: true),
                    RewardCode = table.Column<string>(nullable: true),
                    RewardName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedemptionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedemptionItems_Redemptions_RedemptionId",
                        column: x => x.RedemptionId,
                        principalTable: "Redemptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AddBy = table.Column<string>(nullable: true),
                    AddDate = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    RedemptionItemId = table.Column<int>(nullable: true),
                    RewardType = table.Column<string>(nullable: true),
                    SerialNo = table.Column<string>(nullable: true)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_RedemptionItems_RedemptionId",
                table: "RedemptionItems",
                column: "RedemptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_RedemptionItemId",
                table: "Rewards",
                column: "RedemptionItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rewards");

            migrationBuilder.DropTable(
                name: "RedemptionItems");

            migrationBuilder.DropTable(
                name: "Redemptions");
        }
    }
}
