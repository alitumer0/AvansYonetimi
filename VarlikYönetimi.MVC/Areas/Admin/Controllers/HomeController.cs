using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Context;
using System.Linq;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userCount = await _context.Users.CountAsync();
            var requestCount = await _context.AdvanceRequests.CountAsync();
            var pendingApprovalCount = await _context.AdvanceRequests
                .Where(r => r.Status == RequestStatus.Pending)
                .CountAsync();
            var legalActionCount = await _context.LegalActions.CountAsync();

            
            var repaymentRequests = await _context.AdvanceRequests
                .Include(r => r.User)
                .Where(r => r.Status == RequestStatus.AvansGeriOdenmeyiBekliyor)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            
            var legalActions = await _context.LegalActions
                .Include(l => l.AdvanceRequest)
                    .ThenInclude(a => a.User)
                .Where(l => l.Status == LegalActionStatus.Active)
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentRequests = await _context.AdvanceRequests
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentUsers = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.CreatedAt,
                    RoleNames = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();

            ViewBag.UserCount = userCount;
            ViewBag.RequestCount = requestCount;
            ViewBag.PendingApprovalCount = pendingApprovalCount;
            ViewBag.LegalActionCount = legalActionCount;
            ViewBag.RepaymentRequests = repaymentRequests;
            ViewBag.LegalActions = legalActions;
            ViewBag.RecentRequests = recentRequests;
            ViewBag.RecentUsers = recentUsers;

            return View();
        }
    }
} 