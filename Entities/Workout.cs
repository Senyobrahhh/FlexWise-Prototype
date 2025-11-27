using System.ComponentModel.DataAnnotations;

namespace FlexWise_Prototype.Entities;

public class Workout
{
    public int Id { get; set; }

    // FK to FitnessProgram
    public int FitnessProgramId { get; set; }
    public FitnessProgram? FitnessProgram { get; set; }
    
    public int DayIndex { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    
    public List<WorkoutExerciseTemplate> Exercises { get; set; } = new();
}