using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexWise_Prototype.Migrations
{
    /// <inheritdoc />
    public partial class AddFitnessProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FitnessPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProgramDuration = table.Column<int>(type: "int", nullable: false),
                    Goal = table.Column<int>(type: "int", nullable: false),
                    CreatorUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FitnessPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FitnessPrograms_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FitnessPrograms_CreatorId",
                table: "FitnessPrograms",
                column: "CreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FitnessPrograms");
        }
    }
}
