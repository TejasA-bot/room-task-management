using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomTaskManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "TaskAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "TaskAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "TaskAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_ApprovedBy",
                table: "TaskAssignments",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAssignments_Users_ApprovedBy",
                table: "TaskAssignments",
                column: "ApprovedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskAssignments_Users_ApprovedBy",
                table: "TaskAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TaskAssignments_ApprovedBy",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "TaskAssignments");
        }
    }
}
