using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("advance-requests")]
        public async Task<IActionResult> GetAdvanceRequestReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? userId = null)
        {
            var report = await _reportService.GenerateAdvanceRequestReportAsync(startDate, endDate, userId);
            return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AvansTalepleriRaporu.xlsx");
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPaymentReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? userId = null)
        {
            var report = await _reportService.GeneratePaymentReportAsync(startDate, endDate, userId);
            return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OdemelerRaporu.xlsx");
        }

        [HttpGet("overdue-payments")]
        public async Task<IActionResult> GetOverduePaymentReport()
        {
            var report = await _reportService.GenerateOverduePaymentReportAsync();
            return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GecikmisOdemelerRaporu.xlsx");
        }

        [HttpGet("approvals")]
        public async Task<IActionResult> GetApprovalReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? approverId = null)
        {
            var report = await _reportService.GenerateApprovalReportAsync(startDate, endDate, approverId);
            return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OnayRaporu.xlsx");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserReport(int userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var report = await _reportService.GenerateUserReportAsync(userId, startDate, endDate);
            return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"KullaniciRaporu_{userId}.xlsx");
        }
    }
} 