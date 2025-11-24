using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexWise_Prototype.Migrations
{
    /// <inheritdoc />
    public partial class AddExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExerciseTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    TargetMuscleGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquipmentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutExerciseTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkoutId = table.Column<int>(type: "int", nullable: false),
                    ExerciseTemplateId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    RecommendedSets = table.Column<int>(type: "int", nullable: false),
                    RecommendedReps = table.Column<int>(type: "int", nullable: false),
                    RecommendedWeight = table.Column<double>(type: "float", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExerciseTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutExerciseTemplates_ExerciseTemplates_ExerciseTemplateId",
                        column: x => x.ExerciseTemplateId,
                        principalTable: "ExerciseTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutExerciseTemplates_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExerciseTemplates_ExerciseTemplateId",
                table: "WorkoutExerciseTemplates",
                column: "ExerciseTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExerciseTemplates_WorkoutId_Order",
                table: "WorkoutExerciseTemplates",
                columns: new[] { "WorkoutId", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutExerciseTemplates");

            migrationBuilder.DropTable(
                name: "ExerciseTemplates");
        }
    }
}
