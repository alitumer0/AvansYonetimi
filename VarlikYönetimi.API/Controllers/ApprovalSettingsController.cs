using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.API.DTOs.ApprovalSettingsDTO;
using AutoMapper;

namespace VarlikYönetimi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApprovalSettingsController : ControllerBase
    {
        private readonly IApprovalSettingsService _approvalSettingsService;

        public ApprovalSettingsController(IApprovalSettingsService approvalSettingsService)
        {
            _approvalSettingsService = approvalSettingsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSettings()
        {
            var settings = await _approvalSettingsService.GetAllSettingsAsync();
            return Ok(settings);
        }

        [HttpGet("by-amount/{amount}")]
        public async Task<IActionResult> GetSettingsByAmount(decimal amount)
        {
            var settings = await _approvalSettingsService.GetSettingsByAmountAsync(amount);
            if (settings == null)
                return NotFound();

            return Ok(settings);
        }

        [HttpGet("by-level/{level}")]
        public async Task<IActionResult> GetSettingsByLevel(int level)
        {
            var settings = await _approvalSettingsService.GetSettingsByLevelAsync((ApprovalLevel)level);
            if (settings == null)
                return NotFound();

            return Ok(settings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSettings([FromBody] ApprovalSettings settings)
        {
            var result = await _approvalSettingsService.CreateSettingsAsync(settings);
            if (!result)
                return BadRequest("Ayarlar oluşturulamadı.");

            return CreatedAtAction(nameof(GetSettingsByLevel), new { level = settings.Level }, settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] ApprovalSettings settings)
        {
            var result = await _approvalSettingsService.UpdateSettingsAsync(settings);
            if (!result)
                return BadRequest("Ayarlar güncellenemedi.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSettings(int id)
        {
            var result = await _approvalSettingsService.DeleteSettingsAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("limits")]
        public async Task<IActionResult> GetLimits()
        {
            var bmLimit = await _approvalSettingsService.GetBmLimitAsync();
            var direktörLimit = await _approvalSettingsService.GetDirektorLimitAsync();
            var gmyLimit = await _approvalSettingsService.GetGmyLimitAsync();

            return Ok(new
            {
                BmLimit = bmLimit,
                DirektorLimit = direktörLimit,
                GmyLimit = gmyLimit
            });
        }

        [HttpPut("limits/bm")]
        public async Task<IActionResult> UpdateBmLimit([FromBody] decimal limit, [FromQuery] int userId)
        {
            await _approvalSettingsService.UpdateBmLimitAsync(limit, userId);
            return NoContent();
        }

        [HttpPut("limits/direktor")]
        public async Task<IActionResult> UpdateDirektorLimit([FromBody] decimal limit, [FromQuery] int userId)
        {
            await _approvalSettingsService.UpdateDirektorLimitAsync(limit, userId);
            return NoContent();
        }

        [HttpPut("limits/gmy")]
        public async Task<IActionResult> UpdateGmyLimit([FromBody] decimal limit, [FromQuery] int userId)
        {
            await _approvalSettingsService.UpdateGmyLimitAsync(limit, userId);
            return NoContent();
        }

        [HttpGet("timeouts")]
        public async Task<IActionResult> GetTimeouts()
        {
            var approvalTimeout = await _approvalSettingsService.GetApprovalTimeoutDaysAsync();
            var paymentTimeout = await _approvalSettingsService.GetPaymentTimeoutDaysAsync();
            var repaymentTimeout = await _approvalSettingsService.GetRepaymentTimeoutDaysAsync();

            return Ok(new
            {
                ApprovalTimeout = approvalTimeout,
                PaymentTimeout = paymentTimeout,
                RepaymentTimeout = repaymentTimeout
            });
        }

        [HttpPut("timeouts/approval")]
        public async Task<IActionResult> UpdateApprovalTimeout([FromBody] int days, [FromQuery] int userId)
        {
            await _approvalSettingsService.UpdateApprovalTimeoutDaysAsync(days, userId);
            return NoContent();
        }

        [HttpPut("timeouts/payment")]
        public async Task<IActionResult> UpdatePaymentTimeout([FromBody] int days, [FromQuery] int userId)
        {
            await _approvalSettingsService.UpdatePaymentTimeoutDaysAsync(days, userId);
            return NoContent();
        }

        [HttpPut("timeouts/repayment")]
        public async Task<IActionResult> UpdateRepaymentTimeout([FromBody] int days, [FromQuery] int userId)
        {
            await _approvalSettingsService.UpdateRepaymentTimeoutDaysAsync(days, userId);
            return NoContent();
        }
    }
} 