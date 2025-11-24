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
    public async Task<IActionResult> Create(Workout workout)
    {
        if (!ModelState.IsValid)
            return View(workout);

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

        var model = new AddExerciseToWorkoutModel
        {
            WorkoutId = workoutId,
            Order = 1,
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

        var link = new WorkoutExerciseTemplate
        {
            WorkoutId = model.WorkoutId,
            ExerciseTemplateId = model.ExerciseTemplateId,
            Order = model.Order,
            RecommendedSets = model.RecommendedSets,
            RecommendedReps = model.RecommendedReps,
            RecommendedWeight = model.RecommendedWeight,
            Notes = model.Notes
        };

        _context.WorkoutExerciseTemplates.Add(link);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "FitnessPrograms",
            new { id = (await _context.Workouts.FindAsync(model.WorkoutId))!.FitnessProgramId });
    }
}