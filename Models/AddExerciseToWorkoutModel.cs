using FlexWise_Prototype.Entities;

namespace FlexWise_Prototype.Models;

public class AddExerciseToWorkoutModel
{
    public int WorkoutId { get; set; }
    public int ExerciseTemplateId { get; set; }
    
    public int RecommendedSets { get; set; } = 3;
    public int RecommendedReps { get; set; } = 10;
    public double? RecommendedWeight { get; set; }
    public string? Notes { get; set; }
    
    public List<ExerciseTemplate>? AvailableExercises { get; set; }
}