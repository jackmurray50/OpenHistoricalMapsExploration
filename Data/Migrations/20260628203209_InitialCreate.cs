using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChangesetId = table.Column<long>(type: "bigint", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OsmTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OsmTags_OsmEntity_EntityId",
                        column: x => x.EntityId,
                        principalTable: "OsmEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelationMembers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RelationId = table.Column<long>(type: "bigint", nullable: false),
                    MemberId = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    OsmNodeId = table.Column<long>(type: "bigint", nullable: true),
                    OsmWayId = table.Column<long>(type: "bigint", nullable: true)
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
                name: "WayNodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WayId = table.Column<long>(type: "bigint", nullable: false),
                    NodeId = table.Column<long>(type: "bigint", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_OsmTags_EntityId",
                table: "OsmTags",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_OsmTags_Key",
                table: "OsmTags",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_OsmTags_Key_Value",
                table: "OsmTags",
                columns: new[] { "Key", "Value" });

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
                name: "OsmTags");

            migrationBuilder.DropTable(
                name: "RelationMembers");

            migrationBuilder.DropTable(
                name: "WayNodes");

            migrationBuilder.DropTable(
                name: "OsmEntity");
        }
    }
}
