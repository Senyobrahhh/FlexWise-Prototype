using FlexWise_Prototype.Data;
using FlexWise_Prototype.Entities;
using FlexWise_Prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexWise_Prototype.Controllers;

[Authorize]
public class WorkoutsController : Controller
{
    
    // Dependancy Injection
    private readonly ApplicationDbContext _context;

    public WorkoutsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: /Workouts/Create?programId=1
    public IActionResult Create(int programId)
    {
        ViewBag.ProgramId = programId;
        return View();
    }
    
    // POST: /Workouts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Workout workout)
    {
        if (!ModelState.IsValid)
            return View(workout);
        
        var maxDayIndex = await _context.Workouts
            .Where(w => w.FitnessProgramId == workout.FitnessProgramId)
            .MaxAsync(w => (int?)w.DayIndex) ?? 0;

        workout.DayIndex = maxDayIndex + 1;

        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "FitnessPrograms",
            new { id = workout.FitnessProgramId });
    }
    
    
    // GET: /Workouts/AddExercise?workoutId=1
    public async Task<IActionResult> AddExercise(int workoutId)
    {
        var workout = await _context.Workouts.FindAsync(workoutId);
        if (workout == null) return NotFound();

        var exercises = await _context.ExerciseTemplates.ToListAsync();

        // 👇 Guard: no templates => redirect user
        if (!exercises.Any())
        {
            TempData["ErrorMessage"] = "You must create at least one exercise template before adding exercises to a workout.";
            return RedirectToAction("Create", "ExerciseTemplates");
        }

        var model = new AddExerciseToWorkoutModel
        {
            WorkoutId = workoutId,
            RecommendedSets = 3,
            RecommendedReps = 12,
            AvailableExercises = exercises
        };

        return View(model);
    }

    // POST: /Workouts/AddExercise
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExercise(AddExerciseToWorkoutModel model)
    {
        // Check that the selected exercise actually exists
        var exerciseExists = await _context.ExerciseTemplates
            .AnyAsync(e => e.Id == model.ExerciseTemplateId);

        if (!exerciseExists)
        {
            ModelState.AddModelError("ExerciseTemplateId", "Selected exercise does not exist. Please create an exercise template first.");
            model.AvailableExercises = await _context.ExerciseTemplates.ToListAsync();
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            model.AvailableExercises = await _context.ExerciseTemplates.ToListAsync();
            return View(model);
        }

        var maxOrder = await _context.WorkoutExerciseTemplates
            .Where(x => x.WorkoutId == model.WorkoutId)
            .MaxAsync(x => (int?)x.Order) ?? 0;

        var link = new WorkoutExerciseTemplate
        {
            WorkoutId = model.WorkoutId,
            ExerciseTemplateId = model.ExerciseTemplateId,
            Order = maxOrder + 1,
            RecommendedSets = model.RecommendedSets,
            RecommendedReps = model.RecommendedReps,
            RecommendedWeight = model.RecommendedWeight,
            Notes = model.Notes
        };

        _context.WorkoutExerciseTemplates.Add(link);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Workouts", new { id = model.WorkoutId });
    }
    
    // GET: /Workouts/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var workout = await _context.Workouts
            .Include(w => w.FitnessProgram)
            .Include(w => w.Exercises)
            .ThenInclude(we => we.ExerciseTemplate)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null) return NotFound();

        return View(workout);
    }
    
    // POST: /Workouts/RemoveExercise
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveExercise(int id)
    {
        var link = await _context.WorkoutExerciseTemplates
            .Include(x => x.Workout)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (link == null)
            return NotFound();

        var workoutId = link.WorkoutId;

        _context.WorkoutExerciseTemplates.Remove(link);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = workoutId });
    }
    
    // POST: /Workouts/RemoveWorkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveWorkout(int id)
    {
        // Load the workout and its exercise links
        var workout = await _context.Workouts
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null)
            return NotFound();

        var programId = workout.FitnessProgramId;

        // 1) Remove all sessions + sets for this workout
        var sessions = await _context.WorkoutSessions
            .Where(s => s.WorkoutId == id)
            .Include(s => s.Sets)
            .ToListAsync();

        if (sessions.Count > 0)
        {
            var sets = sessions.SelectMany(s => s.Sets).ToList();
            if (sets.Count > 0)
            {
                _context.SetLogs.RemoveRange(sets);
            }

            _context.WorkoutSessions.RemoveRange(sessions);
        }

        // 2) Remove all exercise links for this workout
        if (workout.Exercises.Any())
        {
            _context.WorkoutExerciseTemplates.RemoveRange(workout.Exercises);
        }

        // 3) Remove the workout itself
        _context.Workouts.Remove(workout);

        await _context.SaveChangesAsync();

        // Back to the program details
        return RedirectToAction("Details", "FitnessPrograms", new { id = programId });
    }
    
    // GET: /Workouts/EditExercise/5
    public async Task<IActionResult> EditExercise(int id)
    {
        var link = await _context.WorkoutExerciseTemplates
            .Include(x => x.ExerciseTemplate)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (link == null) return NotFound();

        var exercises = await _context.ExerciseTemplates.ToListAsync();

        var model = new EditWorkoutExerciseModel
        {
            Id = link.Id,
            WorkoutId = link.WorkoutId,
            ExerciseTemplateId = link.ExerciseTemplateId,
            AvailableExercises = exercises,
            RecommendedSets = link.RecommendedSets,
            RecommendedReps = link.RecommendedReps,
            RecommendedWeight = link.RecommendedWeight,
            Notes = link.Notes
        };

        return View(model);
    }

    // POST: /Workouts/EditExercise
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditExercise(EditWorkoutExerciseModel model)
    {
        // ensure the selected exercise still exists
        var exerciseExists = await _context.ExerciseTemplates
            .AnyAsync(e => e.Id == model.ExerciseTemplateId);

        if (!exerciseExists)
        {
            ModelState.AddModelError("ExerciseTemplateId", "Selected exercise does not exist.");
        }

        if (!ModelState.IsValid)
        {
            model.AvailableExercises = await _context.ExerciseTemplates.ToListAsync();
            return View(model);
        }

        var link = await _context.WorkoutExerciseTemplates
            .FirstOrDefaultAsync(x => x.Id == model.Id);

        if (link == null) return NotFound();

        link.ExerciseTemplateId = model.ExerciseTemplateId;        // 👈 change exercise
        link.RecommendedSets = model.RecommendedSets;
        link.RecommendedReps = model.RecommendedReps;
        link.RecommendedWeight = model.RecommendedWeight;
        link.Notes = model.Notes;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = link.WorkoutId });
    }
}