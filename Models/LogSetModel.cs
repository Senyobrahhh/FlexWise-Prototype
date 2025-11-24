namespace FlexWise_Prototype.Models;

public class LogSetModel
{
    public int WorkoutSessionId { get; set; }
    public int ExerciseTemplateId { get; set; }

    public int SetNumber { get; set; } = 1;
    public int Reps { get; set; }
    public double? Weight { get; set; }
    public string? Notes { get; set; }
}