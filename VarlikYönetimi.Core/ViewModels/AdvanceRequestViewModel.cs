using System;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Enums;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VarlikYönetimi.Core.ViewModels
{
    public class AdvanceRequestViewModel
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string ProjectName { get; set; }
        
        [Required(ErrorMessage = "Miktar alanı zorunludur.")]
        [Range(1, double.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "İstenen tarih alanı zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime DesiredDate { get; set; }
        
        public decimal? ApprovedAmount { get; set; }
        public RequestStatus Status { get; set; }
        public ApprovalLevel CurrentLevel { get; set; }
        public string RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string RequesterName { get; set; }
        public string StatusBadgeClass { get; set; }
        public string StatusText { get; set; }
        public bool CanApprove { get; set; }
        public List<ApprovalHistoryViewModel> ApprovalHistory { get; set; }
        public DateTime? RepaymentDueDate { get; set; }
    }

    public class ApprovalHistoryViewModel
    {
        public string ApproverName { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActionText { get; set; }
        public string UserName { get; set; }
    }

    public class AdvanceRequestListViewModel
    {
        public IEnumerable<AdvanceRequestViewModel> AdvanceRequests { get; set; }
        public int UnreadNotificationCount { get; set; }
    }

    public class AdvanceRequestCreateViewModel
    {
        [Required(ErrorMessage = "Tutar alanı zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Lütfen geçerli bir tutar giriniz.")]
        [Display(Name = "Talep Edilen Tutar")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Açıklama en az {2} en fazla {1} karakter olmalıdır.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "İstenilen tarih alanı zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "İstenilen Tarih")]
        public DateTime DesiredDate { get; set; }

        [Display(Name = "İlgili Projeler")]
        public List<int> ProjectIds { get; set; } = new List<int>();

        [Display(Name = "Belgeler")]
        public List<IFormFile> Documents { get; set; } = new List<IFormFile>();
    }

    public class AdvanceRequestEditViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Miktar alanı zorunludur.")]
        [Range(1, double.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "İstenen tarih alanı zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime DesiredDate { get; set; }
    }
} 