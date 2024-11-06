using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class UserReportService : IUserReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<UserReport>> GetAllReports()
        {
            var reports = await _unitOfWork.RepositoryUserReport.GetData();
            return reports.ToList();
        }

        public async Task<UserReport?> GetReportById(int reportId)
        {
            var reportEntity = await _unitOfWork.RepositoryUserReport.GetById(reportId);
            return reportEntity == null ? null : _mapper.Map<UserReport>(reportEntity);
        }

        public async Task<UserReport> CreateReport(UserReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var reportEntity = _mapper.Map<UserReport>(report);
            await _unitOfWork.RepositoryUserReport.Insert(reportEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserReport>(reportEntity);
        }

        public async Task<bool> UpdateReport(UserReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var reportEntity = await _unitOfWork.RepositoryUserReport.GetById(report.Id);
            if (reportEntity == null) return false;

            reportEntity.Status = report.Status;
            reportEntity.ReportDate = report.ReportDate;
            reportEntity.UserId = report.UserId;
            reportEntity.UserReportId = report.UserReportId;

            _unitOfWork.RepositoryUserReport.Update(reportEntity);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<ServiceResponseModel<bool>> DeleteReport(int reportId)
        {
            var reportEntity = await _unitOfWork.RepositoryUserReport.GetById(reportId);
            if (reportEntity == null)
            {
                return new ServiceResponseModel<bool>(false, "No reports found");
            }

            _unitOfWork.RepositoryUserReport.Delete(reportEntity);
            await _unitOfWork.CommitAsync();

            return new ServiceResponseModel<bool>(true);
        }

        // Thêm phương thức để ánh xạ UserReport
        public UserReport MapToUserReport(UserReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            return _mapper.Map<UserReport>(report);
        }
    }
}
