using System;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.ViewModels
{
    public class ApprovalProcessViewModel
    {
        public int Id { get; set; }
        public int AdvanceRequestId { get; set; }
        public string RequestNumber { get; set; }
        public string RequesterName { get; set; }
        public decimal Amount { get; set; }
        public ApprovalLevel CurrentLevel { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Comments { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }

    public class ApprovalProcessCreateViewModel
    {
        [Required(ErrorMessage = "Avans talebi zorunludur.")]
        public int AdvanceRequestId { get; set; }

        [Required(ErrorMessage = "Onaylayan kullanıcı zorunludur.")]
        public int ApproverUserId { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public ApprovalStatus Status { get; set; }

        public string Comments { get; set; }
    }

    public class ApprovalProcessEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Avans talebi zorunludur.")]
        public int AdvanceRequestId { get; set; }

        [Required(ErrorMessage = "Onaylayan kullanıcı zorunludur.")]
        public int ApproverUserId { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public ApprovalStatus Status { get; set; }

        public string Comments { get; set; }
    }
} 