using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Context;

namespace VarlikYönetimi.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly AppDbContext _context;

        public SettingsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSettings(Settings settings)
        {
            if (ModelState.IsValid)
            {
                var existingSettings = await _context.Settings.FirstOrDefaultAsync();
                if (existingSettings == null)
                {
                    _context.Settings.Add(settings);
                }
                else
                {
                    _context.Entry(existingSettings).CurrentValues.SetValues(settings);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(settings);
        }
    }
} 