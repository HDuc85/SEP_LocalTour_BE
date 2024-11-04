using LocalTour.Domain.Entities;
using LocalTour.Services.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IUserReportService
    {
        Task<List<UserReport>> GetAllReports();
        Task<UserReport?> GetReportById(int reportId);
        Task<UserReport> CreateReport(UserReport report);
        Task<bool> UpdateReport(UserReport report);
        Task<ServiceResponseModel<bool>> DeleteReport(int reportId);
    }
}
