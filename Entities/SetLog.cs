namespace FlexWise_Prototype.Entities;

public class SetLog
{
    public int Id { get; set; }

    public int WorkoutSessionId { get; set; }
    public WorkoutSession? WorkoutSession { get; set; }

    public int ExerciseTemplateId { get; set; }
    public ExerciseTemplate? ExerciseTemplate { get; set; }

    public int SetNumber { get; set; } = 1;
    public int Reps { get; set; }
    public double? Weight { get; set; }
    public string? Notes { get; set; }
}