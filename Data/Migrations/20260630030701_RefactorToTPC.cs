using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToTPC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OsmNode_OsmEntity_Id",
                table: "OsmNode");

            migrationBuilder.DropForeignKey(
                name: "FK_OsmRelation_OsmEntity_Id",
                table: "OsmRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_OsmTags_OsmEntity_EntityId",
                table: "OsmTags");

            migrationBuilder.DropForeignKey(
                name: "FK_OsmWay_OsmEntity_Id",
                table: "OsmWay");

            migrationBuilder.DropForeignKey(
                name: "FK_RelationMembers_OsmEntity_MemberId",
                table: "RelationMembers");

            migrationBuilder.DropTable(
                name: "OsmEntity");

            migrationBuilder.DropIndex(
                name: "IX_OsmTags_EntityId",
                table: "OsmTags");

            migrationBuilder.AddColumn<string>(
                name: "MemberType",
                table: "RelationMembers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ChangesetId",
                table: "OsmWay",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "OsmWay",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "OsmWay",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "OsmWay",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "OsmWay",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "OsmTags",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OsmNodeId",
                table: "OsmTags",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OsmRelationId",
                table: "OsmTags",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OsmWayId",
                table: "OsmTags",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChangesetId",
                table: "OsmRelation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "OsmRelation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "OsmRelation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "OsmRelation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "OsmRelation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ChangesetId",
                table: "OsmNode",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "OsmNode",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "OsmNode",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "OsmNode",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "OsmNode",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RelationMembers_MemberId_MemberType",
                table: "RelationMembers",
                columns: new[] { "MemberId", "MemberType" });

            migrationBuilder.CreateIndex(
                name: "IX_OsmTags_EntityId_EntityType",
                table: "OsmTags",
                columns: new[] { "EntityId", "EntityType" });

            migrationBuilder.CreateIndex(
                name: "IX_OsmTags_OsmNodeId",
                table: "OsmTags",
                column: "OsmNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_OsmTags_OsmRelationId",
                table: "OsmTags",
                column: "OsmRelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OsmTags_OsmWayId",
                table: "OsmTags",
                column: "OsmWayId");

            migrationBuilder.AddForeignKey(
                name: "FK_OsmTags_OsmNode_OsmNodeId",
                table: "OsmTags",
                column: "OsmNodeId",
                principalTable: "OsmNode",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OsmTags_OsmRelation_OsmRelationId",
                table: "OsmTags",
                column: "OsmRelationId",
                principalTable: "OsmRelation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OsmTags_OsmWay_OsmWayId",
                table: "OsmTags",
                column: "OsmWayId",
                principalTable: "OsmWay",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OsmTags_OsmNode_OsmNodeId",
                table: "OsmTags");

            migrationBuilder.DropForeignKey(
                name: "FK_OsmTags_OsmRelation_OsmRelationId",
                table: "OsmTags");

            migrationBuilder.DropForeignKey(
                name: "FK_OsmTags_OsmWay_OsmWayId",
                table: "OsmTags");

            migrationBuilder.DropIndex(
                name: "IX_RelationMembers_MemberId_MemberType",
                table: "RelationMembers");

            migrationBuilder.DropIndex(
                name: "IX_OsmTags_EntityId_EntityType",
                table: "OsmTags");

            migrationBuilder.DropIndex(
                name: "IX_OsmTags_OsmNodeId",
                table: "OsmTags");

            migrationBuilder.DropIndex(
                name: "IX_OsmTags_OsmRelationId",
                table: "OsmTags");

            migrationBuilder.DropIndex(
                name: "IX_OsmTags_OsmWayId",
                table: "OsmTags");

            migrationBuilder.DropColumn(
                name: "MemberType",
                table: "RelationMembers");

            migrationBuilder.DropColumn(
                name: "ChangesetId",
                table: "OsmWay");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "OsmWay");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OsmWay");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "OsmWay");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "OsmWay");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "OsmTags");

            migrationBuilder.DropColumn(
                name: "OsmNodeId",
                table: "OsmTags");

            migrationBuilder.DropColumn(
                name: "OsmRelationId",
                table: "OsmTags");

            migrationBuilder.DropColumn(
                name: "OsmWayId",
                table: "OsmTags");

            migrationBuilder.DropColumn(
                name: "ChangesetId",
                table: "OsmRelation");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "OsmRelation");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OsmRelation");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "OsmRelation");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "OsmRelation");

            migrationBuilder.DropColumn(
                name: "ChangesetId",
                table: "OsmNode");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "OsmNode");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OsmNode");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "OsmNode");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "OsmNode");

            migrationBuilder.CreateTable(
                name: "OsmEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChangesetId = table.Column<long>(type: "bigint", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OsmTags_EntityId",
                table: "OsmTags",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_OsmNode_OsmEntity_Id",
                table: "OsmNode",
                column: "Id",
                principalTable: "OsmEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OsmRelation_OsmEntity_Id",
                table: "OsmRelation",
                column: "Id",
                principalTable: "OsmEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OsmTags_OsmEntity_EntityId",
                table: "OsmTags",
                column: "EntityId",
                principalTable: "OsmEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OsmWay_OsmEntity_Id",
                table: "OsmWay",
                column: "Id",
                principalTable: "OsmEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RelationMembers_OsmEntity_MemberId",
                table: "RelationMembers",
                column: "MemberId",
                principalTable: "OsmEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
