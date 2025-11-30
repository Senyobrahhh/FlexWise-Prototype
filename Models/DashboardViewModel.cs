using FlexWise_Prototype.Entities;

namespace FlexWise_Prototype.Models;

public class DashboardViewModel
{
    public string? DisplayName { get; set; }

    public int TotalWorkoutsCompleted { get; set; }
    public DateTime? LastWorkoutDate { get; set; }

    public List<FitnessProgram> MyPrograms { get; set; } = new();
    public List<WorkoutSession> RecentSessions { get; set; } = new();
}