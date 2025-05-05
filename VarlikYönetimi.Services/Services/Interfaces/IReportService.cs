using System;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateAdvanceRequestReportAsync(DateTime startDate, DateTime endDate, int? userId = null);
        Task<byte[]> GeneratePaymentReportAsync(DateTime startDate, DateTime endDate, int? userId = null);
        Task<byte[]> GenerateOverduePaymentReportAsync();
        Task<byte[]> GenerateApprovalReportAsync(DateTime startDate, DateTime endDate, int? approverId = null);
        Task<byte[]> GenerateUserReportAsync(int userId, DateTime startDate, DateTime endDate);
    }
} 