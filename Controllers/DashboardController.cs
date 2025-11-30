using System.Security.Claims;
using FlexWise_Prototype.Data;
using FlexWise_Prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexWise_Prototype.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Dashboard
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // basic user info
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        // my programs
        var programs = await _context.FitnessPrograms
            .Where(p => p.CreatorUserId == userId)
            .OrderBy(p => p.Title)
            .ToListAsync();

        // Base query for this user's sessions (+ legacy null-user sessions)
        var sessionsQuery = _context.WorkoutSessions
            .Include(s => s.Workout)
            .ThenInclude(w => w.FitnessProgram)
            .AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            sessionsQuery = sessionsQuery.Where(s => s.UserId == userId || s.UserId == null);
        }

        // Recent 5 sessions
        var recentSessions = await sessionsQuery
            .OrderByDescending(s => s.StartTime)
            .Take(5)
            .ToListAsync();

        // Overview numbers
        var totalWorkouts = await sessionsQuery.CountAsync();
        var lastWorkoutDate = recentSessions.FirstOrDefault()?.StartTime;

        var model = new DashboardViewModel
        {
            DisplayName = user?.DisplayName ?? user?.Email,
            MyPrograms = programs,
            RecentSessions = recentSessions,
            TotalWorkoutsCompleted = totalWorkouts,
            LastWorkoutDate = lastWorkoutDate
        };

        return View(model);
    }
}