using System.Security.Claims;
using FlexWise_Prototype.Data;
using FlexWise_Prototype.Entities;
using FlexWise_Prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexWise_Prototype.Controllers;

[Authorize]
public class WorkoutSessionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public WorkoutSessionsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: /WorkoutSessions/Start?workoutId=1
    [Authorize]
    public async Task<IActionResult> Start(int workoutId)
    {
        var workout = await _context.Workouts
            .Include(w => w.Exercises)
            .ThenInclude(we => we.ExerciseTemplate)
            .FirstOrDefaultAsync(w => w.Id == workoutId);

        if (workout == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var session = new WorkoutSession
        {
            WorkoutId = workoutId,
            StartTime = DateTime.UtcNow,
            // later: UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        _context.WorkoutSessions.Add(session);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Log), new { id = session.Id });
    }
    
    // GET: /WorkoutSessions/Log/5
    public async Task<IActionResult> Log(int id)
    {
        var session = await _context.WorkoutSessions
            .Include(s => s.Workout)
            .ThenInclude(w => w.Exercises)
            .ThenInclude(we => we.ExerciseTemplate)
            .Include(s => s.Sets)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) return NotFound();

        ViewBag.Session = session;
        return View(new LogSetModel
        {
            WorkoutSessionId = session.Id,
            SetNumber = session.Sets.Count + 1
        });
    }
    
    // POST: /WorkoutSessions/AddSet
    [HttpPost]
    public async Task<IActionResult> AddSet(LogSetModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Log), new { id = model.WorkoutSessionId });
        }

        var set = new SetLog
        {
            WorkoutSessionId = model.WorkoutSessionId,
            ExerciseTemplateId = model.ExerciseTemplateId,
            SetNumber = model.SetNumber,
            Reps = model.Reps,
            Weight = model.Weight,
            Notes = model.Notes
        };

        _context.SetLogs.Add(set);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Log), new { id = model.WorkoutSessionId });
    }

    // POST: /WorkoutSessions/Finish/5
    [HttpPost]
    public async Task<IActionResult> Finish(int id)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session == null) return NotFound();

        session.EndTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // later: redirect to History; for now, go back to program details
        var workout = await _context.Workouts.FindAsync(session.WorkoutId);
        return RedirectToAction("Details", "FitnessPrograms", new { id = workout!.FitnessProgramId });
    }
    
    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var sessions = await _context.WorkoutSessions
            .Include(s => s.Workout)
            .ThenInclude(w => w.FitnessProgram)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();

        return View(sessions);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ClearHistory()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var sessions = await _context.WorkoutSessions
            .Where(s => s.UserId == userId || s.UserId == null)
            .Include(s => s.Sets)
            .ToListAsync();

        if (sessions.Count == 0)
            return RedirectToAction(nameof(History));

        var sets = sessions.SelectMany(s => s.Sets).ToList();
        if (sets.Count > 0)
        {
            _context.SetLogs.RemoveRange(sets);
        }

        _context.WorkoutSessions.RemoveRange(sessions);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(History));
    }
}