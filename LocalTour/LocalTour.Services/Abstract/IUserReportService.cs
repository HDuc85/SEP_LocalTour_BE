using LocalTour.Domain.Entities;
using LocalTour.Services.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract
{
    public interface IUserReportService
    {
        Task<List<UserReport>> GetAllReports();
        Task<UserReport?> GetReportById(int reportId);
        Task<UserReport> CreateReport(UserReportRequest report, String userReportId);
        Task<bool> UpdateReport(UserReport report);
        Task<ServiceResponseModel<bool>> DeleteReport(int reportId);
    }
}
