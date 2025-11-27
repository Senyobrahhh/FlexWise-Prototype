using System.ComponentModel.DataAnnotations.Schema;

namespace FlexWise_Prototype.Entities;

public class WorkoutSession
{
    public int Id { get; set; }

    public int WorkoutId { get; set; }
    public Workout? Workout { get; set; }
    
    public string? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public AppUser? User { get; set; }

    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }

    public List<SetLog> Sets { get; set; } = new();
}