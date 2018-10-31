using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EMRedemption.Data.Migrations
{
    public partial class Addedmodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RedemptionItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Points = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    RewardCode = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedemptionItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Redemptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    FatchDateTime = table.Column<DateTime>(nullable: false),
                    RedeemDateTime = table.Column<DateTime>(nullable: false),
                    RetailerEmailAddress = table.Column<string>(nullable: true),
                    RetailerName = table.Column<string>(nullable: true),
                    RetailerPhoneNumber = table.Column<string>(nullable: true),
                    RetailerStoreName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TrasactionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redemptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AddBy = table.Column<string>(nullable: true),
                    AddDate = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    RedemptionItemId = table.Column<int>(nullable: true),
                    RewardType = table.Column<string>(nullable: true),
                    SerialNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupons_RedemptionItems_RedemptionItemId",
                        column: x => x.RedemptionItemId,
                        principalTable: "RedemptionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_RedemptionItemId",
                table: "Coupons",
                column: "RedemptionItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Redemptions");

            migrationBuilder.DropTable(
                name: "RedemptionItems");
        }
    }
}
