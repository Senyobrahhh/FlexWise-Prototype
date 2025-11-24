using System.ComponentModel.DataAnnotations;

namespace FlexWise_Prototype.Entities;

public enum ProgramDuration
{
    OneWeek = 7,
    TwoWeeks = 14,
    ThreeWeeks = 21,
    OneMonth = 28
}

public enum GoalType
{
    FatLoss,
    MuscleGain,
    Strength,
    GeneralFitness
}

public class FitnessProgram
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(80)]
    public string Title { get; set; }
    
    [MaxLength(250)]
    public string? Description { get; set; }

    public ProgramDuration ProgramDuration { get; set; }
    public GoalType Goal { get; set; }
    
    //Reference to creator
    public string CreatorUserId { get; set; } = "";
    public AppUser? Creator { get; set; }
    
    public bool IsPublic { get; set; } = true;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public List<Workout> Workouts { get; set; } = new();
}