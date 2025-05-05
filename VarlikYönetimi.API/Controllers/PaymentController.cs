using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.API.DTOs.PaymentDTO;
using AutoMapper;

namespace VarlikYönetimi.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public PaymentController(
            IPaymentService paymentService,
            INotificationService notificationService,
            IMapper mapper)
        {
            _paymentService = paymentService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();
            var paymentDTOs = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
            return Ok(paymentDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDTO>> GetById(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
                return NotFound();

            var paymentDTO = _mapper.Map<PaymentDTO>(payment);
            return Ok(paymentDTO);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByUserId(int userId)
        {
            var payments = await _paymentService.GetPaymentsByUserAsync(userId);
            return Ok(payments);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPendingPayments()
        {
            var payments = await _paymentService.GetPendingPaymentsAsync();
            return Ok(payments);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetOverduePayments()
        {
            var payments = await _paymentService.GetOverduePaymentsAsync();
            return Ok(payments);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByStatus(PaymentStatus status)
        {
            var payments = await _paymentService.GetPaymentsByStatusAsync(status);
            return Ok(payments);
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
            return Ok(payments);
        }

        [HttpPost("{advanceRequestId}")]
        public async Task<ActionResult<Payment>> CreatePayment(int advanceRequestId, [FromBody] CreatePaymentRequest request)
        {
            try
            {
                var advanceRequest = new AdvanceRequest { Id = advanceRequestId };
                var payment = await _paymentService.CreatePaymentAsync(advanceRequest, request.EnteredByUserId);
                return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePaymentStatusRequest request)
        {
            try
            {
                await _paymentService.UpdatePaymentStatusAsync(id, request.NewStatus, request.DeliveredByUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/markoverdue")]
        public async Task<IActionResult> MarkAsOverdue(int id)
        {
            try
            {
                await _paymentService.MarkPaymentAsOverdueAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreatePaymentRequest
    {
        public int EnteredByUserId { get; set; }
    }

    public class UpdatePaymentStatusRequest
    {
        public PaymentStatus NewStatus { get; set; }
        public int? DeliveredByUserId { get; set; }
    }
} 