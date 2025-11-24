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
}