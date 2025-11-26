using System.Security.Claims;
using FlexWise_Prototype.Data;
using FlexWise_Prototype.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexWise_Prototype.Controllers;

[Authorize]
public class FitnessProgramsController : Controller
{
    
    //Dependancy Injection
    private readonly ApplicationDbContext _context;

    public FitnessProgramsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: /FitnessPrograms
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var programs = await _context.FitnessPrograms
            .Where(p => p.CreatorUserId == userId)  // show only my programs
            .Include(p => p.Creator)                // load Creator for DisplayName
            .ToListAsync();

        return View(programs);
    }
    
    // GET: /FitnessPrograms/Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: /FitnessPrograms/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FitnessProgram program)
    {
        if (!ModelState.IsValid)
            return View(program);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        program.CreatorUserId = userId;
        program.DateCreated = DateTime.UtcNow;

        _context.FitnessPrograms.Add(program);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    //POST: /FitnessPrograms/Remove
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProgram(int id)
    {
        var program = await _context.FitnessPrograms
            .Include(p => p.Workouts)
            .ThenInclude(w => w.Exercises)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (program == null)
            return NotFound();

        // ensure only the creator can delete
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (program.CreatorUserId != null && program.CreatorUserId != userId)
            return Forbid();

        _context.FitnessPrograms.Remove(program);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var program = await _context.FitnessPrograms
            .Include(p => p.Creator)
            .Include(p => p.Workouts)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (program == null) return NotFound();

        return View(program);
    }

}