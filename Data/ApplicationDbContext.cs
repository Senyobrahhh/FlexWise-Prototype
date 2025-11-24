using FlexWise_Prototype.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlexWise_Prototype.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<FitnessProgram> FitnessPrograms => Set<FitnessProgram>();
    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<ExerciseTemplate> ExerciseTemplates => Set<ExerciseTemplate>();
    public DbSet<WorkoutExerciseTemplate> WorkoutExerciseTemplates => Set<WorkoutExerciseTemplate>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<SetLog> SetLogs => Set<SetLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<WorkoutExerciseTemplate>()
            .HasIndex(x => new { x.WorkoutId, x.Order })
            .IsUnique();
        
        builder.Entity<SetLog>()
            .HasIndex(x => new { x.WorkoutSessionId, x.ExerciseTemplateId, x.SetNumber });
    }
}