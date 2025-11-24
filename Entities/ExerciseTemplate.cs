using System.ComponentModel.DataAnnotations;

namespace FlexWise_Prototype.Entities;

public class ExerciseTemplate
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = "";
    [MaxLength(120)]
    public string? Description { get; set; }
    public string? TargetMuscleGroup { get; set; }  
    public string? EquipmentType { get; set; }   
}