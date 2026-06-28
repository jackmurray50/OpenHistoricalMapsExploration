using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OsmEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChangesetId = table.Column<long>(type: "INTEGER", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: true),
                    Visible = table.Column<bool>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelationMembers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RelationId = table.Column<long>(type: "INTEGER", nullable: false),
                    MemberId = table.Column<long>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SequenceNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    OsmNodeId = table.Column<long>(type: "INTEGER", nullable: true),
                    OsmWayId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelationMembers_OsmEntity_MemberId",
                        column: x => x.MemberId,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RelationMembers_OsmEntity_OsmNodeId",
                        column: x => x.OsmNodeId,
                        principalTable: "OsmEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RelationMembers_OsmEntity_OsmWayId",
                        column: x => x.OsmWayId,
                        principalTable: "OsmEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RelationMembers_OsmEntity_RelationId",
                        column: x => x.RelationId,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    EntityId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_OsmEntity_EntityId",
                        column: x => x.EntityId,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WayNodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WayId = table.Column<long>(type: "INTEGER", nullable: false),
                    NodeId = table.Column<long>(type: "INTEGER", nullable: false),
                    SequenceNumber = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WayNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WayNodes_OsmEntity_NodeId",
                        column: x => x.NodeId,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WayNodes_OsmEntity_WayId",
                        column: x => x.WayId,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RelationMembers_MemberId",
                table: "RelationMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationMembers_OsmNodeId",
                table: "RelationMembers",
                column: "OsmNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationMembers_OsmWayId",
                table: "RelationMembers",
                column: "OsmWayId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationMembers_RelationId",
                table: "RelationMembers",
                column: "RelationId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationMembers_RelationId_SequenceNumber",
                table: "RelationMembers",
                columns: new[] { "RelationId", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_EntityId",
                table: "Tags",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Key",
                table: "Tags",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Key_Value",
                table: "Tags",
                columns: new[] { "Key", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_WayNodes_NodeId",
                table: "WayNodes",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_WayNodes_WayId",
                table: "WayNodes",
                column: "WayId");

            migrationBuilder.CreateIndex(
                name: "IX_WayNodes_WayId_SequenceNumber",
                table: "WayNodes",
                columns: new[] { "WayId", "SequenceNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelationMembers");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "WayNodes");

            migrationBuilder.DropTable(
                name: "OsmEntity");
        }
    }
}
