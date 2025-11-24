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
        var programs = await _context.FitnessPrograms
            .Include(p => p.Creator)
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
    public async Task<IActionResult> Create(FitnessProgram program)
    {
        if (!ModelState.IsValid)
            return View(program);
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        program.CreatorUserId = User.Identity!.Name!;
        program.DateCreated = DateTime.UtcNow;

        _context.FitnessPrograms.Add(program);
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