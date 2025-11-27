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

        var maxOrder = await _context.WorkoutExerciseTemplates
            .Where(x => x.WorkoutId == workoutId)
            .MaxAsync(x => (int?)x.Order) ?? 0;

        
        var model = new AddExerciseToWorkoutModel
        {
            WorkoutId = workoutId,
            RecommendedSets = 3,
            RecommendedReps = 10,
            AvailableExercises = await _context.ExerciseTemplates.ToListAsync()
        };

        return View(model);
    }

    // POST: /Workouts/AddExercise
    [HttpPost]
    public async Task<IActionResult> AddExercise(AddExerciseToWorkoutModel model)
    {
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

       // return RedirectToAction("Details", "FitnessPrograms",
       //    new { id = (await _context.Workouts.FindAsync(model.WorkoutId))!.FitnessProgramId });
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
}