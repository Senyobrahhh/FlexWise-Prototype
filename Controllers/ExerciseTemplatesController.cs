using FlexWise_Prototype.Data;
using FlexWise_Prototype.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexWise_Prototype.Controllers;

[Authorize]
public class ExerciseTemplatesController : Controller
{
    //Dependancy Injection
    private readonly ApplicationDbContext _context;

    public ExerciseTemplatesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /ExerciseTemplates
    public async Task<IActionResult> Index()
    {
        var exercises = await _context.ExerciseTemplates.ToListAsync();
        return View(exercises);
    }

    // GET: /ExerciseTemplates/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /ExerciseTemplates/Create
    [HttpPost]
    public async Task<IActionResult> Create(ExerciseTemplate model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.ExerciseTemplates.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var template = await _context.ExerciseTemplates.FindAsync(id);
        if (template == null)
            return NotFound();

        // 1) Remove all set logs that reference this exercise
        var sets = await _context.SetLogs
            .Where(s => s.ExerciseTemplateId == id)
            .ToListAsync();

        if (sets.Count > 0)
        {
            _context.SetLogs.RemoveRange(sets);
        }

        // 2) Remove all workout-exercise links that reference this exercise
        var links = await _context.WorkoutExerciseTemplates
            .Where(x => x.ExerciseTemplateId == id)
            .ToListAsync();

        if (links.Count > 0)
        {
            _context.WorkoutExerciseTemplates.RemoveRange(links);
        }

        // 3) Remove the exercise template itself
        _context.ExerciseTemplates.Remove(template);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
}