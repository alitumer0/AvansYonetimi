using System.Threading.Tasks;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IBackgroundJobService
    {
        Task CheckPendingApprovalsAsync();
        Task CheckPaymentDueDatesAsync();
        Task CheckOverduePaymentsAsync();
        Task SendWeeklyRemindersAsync();
        Task ProcessExpiredRequestsAsync();
    }
} 