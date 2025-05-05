using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IAdvanceRequestService _advanceRequestService;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;

        public ReportService(
            IAdvanceRequestService advanceRequestService,
            IPaymentService paymentService,
            IUserService userService)
        {
            _advanceRequestService = advanceRequestService;
            _paymentService = paymentService;
            _userService = userService;
        }

        public async Task<byte[]> GenerateAdvanceRequestReportAsync(DateTime startDate, DateTime endDate, int? userId = null)
        {
            var requests = userId.HasValue
                ? await _advanceRequestService.GetUserAdvanceRequestsAsync(userId.Value)
                : await _advanceRequestService.GetAllAsync();

            requests = requests.Where(r => r.RequestDate >= startDate && r.RequestDate <= endDate).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Avans Talepleri");
                
                worksheet.Cell(1, 1).Value = "Talep No";
                worksheet.Cell(1, 2).Value = "Kullanıcı";
                worksheet.Cell(1, 3).Value = "Tutar";
                worksheet.Cell(1, 4).Value = "Talep Tarihi";
                worksheet.Cell(1, 5).Value = "İstenen Tarih";
                worksheet.Cell(1, 6).Value = "Durum";
                worksheet.Cell(1, 7).Value = "Açıklama";

                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var request in requests)
                {
                    worksheet.Cell(row, 1).Value = request.Id;
                    worksheet.Cell(row, 2).Value = request.User?.FirstName ?? "Bilinmiyor";
                    worksheet.Cell(row, 3).Value = request.Amount;
                    worksheet.Cell(row, 4).Value = request.RequestDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 5).Value = request.DesiredDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 6).Value = request.Status.ToString();
                    worksheet.Cell(row, 7).Value = request.Description;
                    row++;
                }

                var tableRange = worksheet.Range(1, 1, row - 1, 7);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> GeneratePaymentReportAsync(DateTime startDate, DateTime endDate, int? userId = null)
        {
            var payments = userId.HasValue
                ? await _paymentService.GetPaymentsByUserAsync(userId.Value)
                : await _paymentService.GetAllAsync();

            payments = payments.Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Ödemeler");
                
                worksheet.Cell(1, 1).Value = "Ödeme No";
                worksheet.Cell(1, 2).Value = "Talep No";
                worksheet.Cell(1, 3).Value = "Kullanıcı";
                worksheet.Cell(1, 4).Value = "Tutar";
                worksheet.Cell(1, 5).Value = "Ödeme Tarihi";
                worksheet.Cell(1, 6).Value = "Durum";
                worksheet.Cell(1, 7).Value = "Fiş No";

                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var payment in payments)
                {
                    worksheet.Cell(row, 1).Value = payment.Id;
                    worksheet.Cell(row, 2).Value = payment.AdvanceRequestId;
                    worksheet.Cell(row, 3).Value = payment.AdvanceRequest?.User?.FirstName ?? "Bilinmiyor";
                    worksheet.Cell(row, 4).Value = payment.Amount;
                    worksheet.Cell(row, 5).Value = payment.PaymentDate?.ToString("dd/MM/yyyy") ?? "-";
                    worksheet.Cell(row, 6).Value = payment.Status.ToString();
                    worksheet.Cell(row, 7).Value = payment.ReceiptNumber ?? "-";
                    row++;
                }

                var tableRange = worksheet.Range(1, 1, row - 1, 7);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> GenerateOverduePaymentReportAsync()
        {
            var payments = await _paymentService.GetOverduePaymentsAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Gecikmiş Ödemeler");
                
                worksheet.Cell(1, 1).Value = "Ödeme No";
                worksheet.Cell(1, 2).Value = "Talep No";
                worksheet.Cell(1, 3).Value = "Kullanıcı";
                worksheet.Cell(1, 4).Value = "Tutar";
                worksheet.Cell(1, 5).Value = "Ödeme Tarihi";
                worksheet.Cell(1, 6).Value = "Gecikme Süresi (Gün)";

                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var payment in payments)
                {
                    worksheet.Cell(row, 1).Value = payment.Id;
                    worksheet.Cell(row, 2).Value = payment.AdvanceRequestId;
                    worksheet.Cell(row, 3).Value = payment.AdvanceRequest?.User?.FirstName ?? "Bilinmiyor";
                    worksheet.Cell(row, 4).Value = payment.Amount;
                    worksheet.Cell(row, 5).Value = payment.PaymentDate?.ToString("dd/MM/yyyy") ?? "-";
                    worksheet.Cell(row, 6).Value = payment.PaymentDate.HasValue ? (DateTime.Now - payment.PaymentDate.Value).Days : 0;
                    row++;
                }

                var tableRange = worksheet.Range(1, 1, row - 1, 6);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> GenerateApprovalReportAsync(DateTime startDate, DateTime endDate, int? approverId = null)
        {
            var requests = approverId.HasValue
                ? await _advanceRequestService.GetPendingApprovalsAsync(approverId.Value)
                : await _advanceRequestService.GetAllAsync();

            requests = requests.Where(r => r.RequestDate >= startDate && r.RequestDate <= endDate).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Onay Süreçleri");
                
                worksheet.Cell(1, 1).Value = "Talep No";
                worksheet.Cell(1, 2).Value = "Kullanıcı";
                worksheet.Cell(1, 3).Value = "Tutar";
                worksheet.Cell(1, 4).Value = "Talep Tarihi";
                worksheet.Cell(1, 5).Value = "Onay Seviyesi";
                worksheet.Cell(1, 6).Value = "Durum";
                worksheet.Cell(1, 7).Value = "Onaylayan";

                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var request in requests)
                {
                    worksheet.Cell(row, 1).Value = request.Id;
                    worksheet.Cell(row, 2).Value = request.User?.FirstName ?? "Bilinmiyor";
                    worksheet.Cell(row, 3).Value = request.Amount;
                    worksheet.Cell(row, 4).Value = request.RequestDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 5).Value = request.CurrentLevel.ToString();
                    worksheet.Cell(row, 6).Value = request.Status.ToString();
                    worksheet.Cell(row, 7).Value = request.Approvals?.LastOrDefault()?.ApproverUser?.FirstName ?? "-";
                    row++;
                }

                var tableRange = worksheet.Range(1, 1, row - 1, 7);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> GenerateUserReportAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var user = await _userService.GetByIdAsync(userId);
            var requests = await _advanceRequestService.GetUserAdvanceRequestsAsync(userId);
            requests = requests.Where(r => r.RequestDate >= startDate && r.RequestDate <= endDate).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Kullanıcı Raporu");
                
                worksheet.Cell(1, 1).Value = "Kullanıcı Adı:";
                worksheet.Cell(1, 2).Value = user?.FirstName ?? "Bilinmiyor";
                worksheet.Cell(2, 1).Value = "Departman:";
                worksheet.Cell(2, 2).Value = user?.UserRoles?.FirstOrDefault()?.Role?.Name ?? "Bilinmiyor";
                worksheet.Cell(3, 1).Value = "Rapor Tarihi:";
                worksheet.Cell(3, 2).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                var headerRange = worksheet.Range(1, 1, 3, 1);
                headerRange.Style.Font.Bold = true;

                worksheet.Cell(5, 1).Value = "Talep No";
                worksheet.Cell(5, 2).Value = "Tutar";
                worksheet.Cell(5, 3).Value = "Talep Tarihi";
                worksheet.Cell(5, 4).Value = "İstenen Tarih";
                worksheet.Cell(5, 5).Value = "Durum";
                worksheet.Cell(5, 6).Value = "Açıklama";

                var tableHeaderRange = worksheet.Range(5, 1, 5, 6);
                tableHeaderRange.Style.Font.Bold = true;
                tableHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 6;
                foreach (var request in requests)
                {
                    worksheet.Cell(row, 1).Value = request.Id;
                    worksheet.Cell(row, 2).Value = request.Amount;
                    worksheet.Cell(row, 3).Value = request.RequestDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 4).Value = request.DesiredDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 5).Value = request.Status.ToString();
                    worksheet.Cell(row, 6).Value = request.Description;
                    row++;
                }

                var tableRange = worksheet.Range(5, 1, row - 1, 6);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
} 