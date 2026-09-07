using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToTablePerType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationMembers_OsmEntity_OsmNodeId",
                table: "RelationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_RelationMembers_OsmEntity_OsmWayId",
                table: "RelationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_RelationMembers_OsmEntity_RelationId",
                table: "RelationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_WayNodes_OsmEntity_NodeId",
                table: "WayNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_WayNodes_OsmEntity_WayId",
                table: "WayNodes");

            migrationBuilder.DropIndex(
                name: "IX_OsmEntity_Latitude",
                table: "OsmEntity");

            migrationBuilder.DropIndex(
                name: "IX_OsmEntity_Latitude_Longitude",
                table: "OsmEntity");

            migrationBuilder.DropIndex(
                name: "IX_OsmEntity_Longitude",
                table: "OsmEntity");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "OsmEntity");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "OsmEntity");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "OsmEntity");

            migrationBuilder.CreateTable(
                name: "OsmNode",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OsmNode_OsmEntity_Id",
                        column: x => x.Id,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OsmRelation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OsmRelation_OsmEntity_Id",
                        column: x => x.Id,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OsmWay",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmWay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OsmWay_OsmEntity_Id",
                        column: x => x.Id,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OsmNode_Latitude_Longitude",
                table: "OsmNode",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.AddForeignKey(
                name: "FK_RelationMembers_OsmNode_OsmNodeId",
                table: "RelationMembers",
                column: "OsmNodeId",
                principalTable: "OsmNode",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationMembers_OsmRelation_RelationId",
                table: "RelationMembers",
                column: "RelationId",
                principalTable: "OsmRelation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RelationMembers_OsmWay_OsmWayId",
                table: "RelationMembers",
                column: "OsmWayId",
                principalTable: "OsmWay",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WayNodes_OsmNode_NodeId",
                table: "WayNodes",
                column: "NodeId",
                principalTable: "OsmNode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WayNodes_OsmWay_WayId",
                table: "WayNodes",
                column: "WayId",
                principalTable: "OsmWay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationMembers_OsmNode_OsmNodeId",
                table: "RelationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_RelationMembers_OsmRelation_RelationId",
                table: "RelationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_RelationMembers_OsmWay_OsmWayId",
                table: "RelationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_WayNodes_OsmNode_NodeId",
                table: "WayNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_WayNodes_OsmWay_WayId",
                table: "WayNodes");

            migrationBuilder.DropTable(
                name: "OsmNode");

            migrationBuilder.DropTable(
                name: "OsmRelation");

            migrationBuilder.DropTable(
                name: "OsmWay");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "OsmEntity",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "OsmEntity",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "OsmEntity",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OsmEntity_Latitude",
                table: "OsmEntity",
                column: "Latitude");

            migrationBuilder.CreateIndex(
                name: "IX_OsmEntity_Latitude_Longitude",
                table: "OsmEntity",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_OsmEntity_Longitude",
                table: "OsmEntity",
                column: "Longitude");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationMembers_OsmEntity_OsmNodeId",
                table: "RelationMembers",
                column: "OsmNodeId",
                principalTable: "OsmEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationMembers_OsmEntity_OsmWayId",
                table: "RelationMembers",
                column: "OsmWayId",
                principalTable: "OsmEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationMembers_OsmEntity_RelationId",
                table: "RelationMembers",
                column: "RelationId",
                principalTable: "OsmEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WayNodes_OsmEntity_NodeId",
                table: "WayNodes",
                column: "NodeId",
                principalTable: "OsmEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WayNodes_OsmEntity_WayId",
                table: "WayNodes",
                column: "WayId",
                principalTable: "OsmEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
