using System.ComponentModel.DataAnnotations;
using FlexWise_Prototype.Entities;

namespace FlexWise_Prototype.Models;

public class EditWorkoutExerciseModel
{
    public int Id { get; set; } 
    public int WorkoutId { get; set; } 

    [Required]
    public int ExerciseTemplateId { get; set; }  

    public List<ExerciseTemplate>? AvailableExercises { get; set; }

    [Range(1, 50)]
    public int RecommendedSets { get; set; }

    [Range(1, 100)]
    public int RecommendedReps { get; set; }

    public double? RecommendedWeight { get; set; }

    public string? Notes { get; set; }
}