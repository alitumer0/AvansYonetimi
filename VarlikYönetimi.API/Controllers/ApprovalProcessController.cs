using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.API.DTOs.ApprovalProcessDTO;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Interfaces;
using AutoMapper;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApprovalProcessController : ControllerBase
    {
        private readonly IApprovalProcessService _approvalProcessService;
        private readonly IMapper _mapper;

        public ApprovalProcessController(IApprovalProcessService approvalProcessService, IMapper mapper)
        {
            _approvalProcessService = approvalProcessService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApprovalProcessDTO>>> GetAll()
        {
            var approvalProcesses = await _approvalProcessService.GetAllApprovalProcessesAsync();
            var approvalProcessDTOs = _mapper.Map<IEnumerable<ApprovalProcessDTO>>(approvalProcesses);
            return Ok(approvalProcessDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApprovalProcessDTO>> GetById(int id)
        {
            var approvalProcess = await _approvalProcessService.GetByIdAsync(id); 
            if (approvalProcess == null)
                return NotFound();

            var approvalProcessDTO = _mapper.Map<ApprovalProcessDTO>(approvalProcess);
            return Ok(approvalProcessDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ApprovalProcessDTO>> Create([FromBody] ApprovalProcessCreateDTO createDTO)
        {
            var approvalProcess = _mapper.Map<ApprovalProcess>(createDTO);
            var createdApprovalProcess = await _approvalProcessService.CreateApprovalProcessAsync(approvalProcess);
            var approvalProcessDTO = _mapper.Map<ApprovalProcessDTO>(createdApprovalProcess);
            return CreatedAtAction(nameof(GetById), new { id = approvalProcessDTO.Id }, approvalProcessDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApprovalProcessDTO>> Update(int id, [FromBody] ApprovalProcessUpdateDTO updateDTO)
        {
            if (id != updateDTO.Id)
                return BadRequest();

            var approvalProcess = _mapper.Map<ApprovalProcess>(updateDTO);
            var updatedApprovalProcess = await _approvalProcessService.UpdateApprovalProcessAsync(approvalProcess);
            var approvalProcessDTO = _mapper.Map<ApprovalProcessDTO>(updatedApprovalProcess);
            return Ok(approvalProcessDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _approvalProcessService.DeleteApprovalProcessAsync(id);
            return NoContent();
        }

        [HttpGet("advance-request/{advanceRequestId}")]
        public async Task<ActionResult<IEnumerable<ApprovalProcessDTO>>> GetByAdvanceRequestId(int advanceRequestId)
        {
            var approvalProcesses = await _approvalProcessService.GetApprovalsByRequestIdAsync(advanceRequestId);
            var approvalProcessDTOs = _mapper.Map<IEnumerable<ApprovalProcessDTO>>(approvalProcesses);
            return Ok(approvalProcessDTOs);
        }

        [HttpGet("approver/{approverUserId}")]
        public async Task<ActionResult<IEnumerable<ApprovalProcessDTO>>> GetByApproverUserId(int approverUserId)
        {
            var approvalProcesses = await _approvalProcessService.GetPendingApprovalsAsync(approverUserId);
            var approvalProcessDTOs = _mapper.Map<IEnumerable<ApprovalProcessDTO>>(approvalProcesses);
            return Ok(approvalProcessDTOs);
        }
    }
} 