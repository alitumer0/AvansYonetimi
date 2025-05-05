using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.API.DTOs.AdvanceRequestDTO;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Services.Services.Interfaces;
using AutoMapper;

namespace VarlikYönetimi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdvanceRequestController : ControllerBase
    {
        private readonly IAdvanceRequestService _advanceRequestService;
        private readonly IApprovalProcessService _approvalProcessService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public AdvanceRequestController(
            IAdvanceRequestService advanceRequestService,
            IApprovalProcessService approvalProcessService,
            INotificationService notificationService,
            IMapper mapper)
        {
            _advanceRequestService = advanceRequestService;
            _approvalProcessService = approvalProcessService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<IEnumerable<AdvanceRequestDTO>>> GetAll()
        {
            var advanceRequests = await _advanceRequestService.GetAllAsync();
            var advanceRequestDTOs = _mapper.Map<IEnumerable<AdvanceRequestDTO>>(advanceRequests);
            return Ok(advanceRequestDTOs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<AdvanceRequestDTO>> GetById(int id)
        {
            var advanceRequest = await _advanceRequestService.GetByIdAsync(id);
            if (advanceRequest == null)
                return NotFound();

            var advanceRequestDTO = _mapper.Map<AdvanceRequestDTO>(advanceRequest);
            return Ok(advanceRequestDTO);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AdvanceRequestDTO>>> GetByUserId(int userId)
        {
            var advanceRequests = await _advanceRequestService.GetByUserIdAsync(userId);
            var advanceRequestDTOs = _mapper.Map<IEnumerable<AdvanceRequestDTO>>(advanceRequests);
            return Ok(advanceRequestDTOs);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<AdvanceRequestDTO>> Create([FromBody] CreateAdvanceRequestDTO createDTO)
        {
            var advanceRequest = _mapper.Map<AdvanceRequest>(createDTO);
            var createdAdvanceRequest = await _advanceRequestService.CreateAsync(advanceRequest);
            var advanceRequestDTO = _mapper.Map<AdvanceRequestDTO>(createdAdvanceRequest);
            return CreatedAtAction(nameof(GetById), new { id = advanceRequestDTO.Id }, advanceRequestDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AdvanceRequestDTO>> Update(int id, [FromBody] UpdateAdvanceRequestDTO updateDTO)
        {
            if (id != updateDTO.Id)
                return BadRequest();

            var advanceRequest = _mapper.Map<AdvanceRequest>(updateDTO);
            var updatedAdvanceRequest = await _advanceRequestService.UpdateAsync(advanceRequest);
            var advanceRequestDTO = _mapper.Map<AdvanceRequestDTO>(updatedAdvanceRequest);
            return Ok(advanceRequestDTO);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _advanceRequestService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApprovalRequest approvalRequest)
        {
            var request = await _advanceRequestService.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var success = await _advanceRequestService.ApproveRequestAsync(id, userId, approvalRequest.ApprovedAmount, approvalRequest.Comments);
            
            if (!success)
                return BadRequest("Onay işlemi başarısız oldu.");

            return NoContent();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectionRequest rejectionRequest)
        {
            var request = await _advanceRequestService.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var success = await _advanceRequestService.RejectRequestAsync(id, userId, rejectionRequest.Comments);
            
            if (!success)
                return BadRequest("Red işlemi başarısız oldu.");

            return NoContent();
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<AdvanceRequestDTO>>> GetByStatus(string status)
        {
            if (!Enum.TryParse<RequestStatus>(status, true, out var parsedStatus))
                return BadRequest("Invalid status value.");

            var advanceRequests = await _advanceRequestService.GetByStatusAsync(parsedStatus);
            var advanceRequestDTOs = _mapper.Map<IEnumerable<AdvanceRequestDTO>>(advanceRequests);
            return Ok(advanceRequestDTOs);
        }
    }

    public class ApprovalRequest
    {
        public decimal ApprovedAmount { get; set; }
        public string Comments { get; set; }
    }

    public class RejectionRequest
    {
        public string Comments { get; set; }
    }
} 