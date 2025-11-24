namespace FlexWise_Prototype.Entities;

public class WorkoutExerciseTemplate
{
    public int Id { get; set; }

    // FK to Workout
    public int WorkoutId { get; set; }
    public Workout? Workout { get; set; }

    // FK to ExerciseTemplate
    public int ExerciseTemplateId { get; set; }
    public ExerciseTemplate? ExerciseTemplate { get; set; }

    // Order in the workout (1, 2, 3...)
    public int Order { get; set; } = 1;

    // Basic prescription
    public int RecommendedSets { get; set; } = 3;
    public int RecommendedReps { get; set; } = 10;
    public double? RecommendedWeight { get; set; }
    public string? Notes { get; set; }
}