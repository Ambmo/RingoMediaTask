using Microsoft.AspNetCore.Mvc;
using RingoMediaTask.Data;
using RingoMediaTask.Models;
using Microsoft.EntityFrameworkCore;

namespace RingoMediaTask.Controllers
{
    public class RemindersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RemindersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Reminders.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,DateTime")] Reminder reminder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reminder);
                await _context.SaveChangesAsync();
                // Trigger email notification logic here
                return RedirectToAction(nameof(Index));
            }
            return View(reminder);
        }
    }
}
