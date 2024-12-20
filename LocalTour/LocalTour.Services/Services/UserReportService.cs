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

        public async Task<UserReport> CreateReport(UserReportRequest report, String userReportId)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            
            Guid xserReportId = Guid.Parse(userReportId);
            
            var userReported = await _unitOfWork.RepositoryUser.GetById(report.UserId);
            if (userReported == null)
            {
                throw new Exception("User report not found");
            }
            
            var existReport =  _unitOfWork.RepositoryUserReport.GetAll().Where(x => x.UserReportId == xserReportId && x.UserId == userReported.Id);
            if (existReport.Any())
            {
                DateTime newestDate = existReport.Max(item => item.ReportDate);
                DateTime now = DateTime.Now;
                TimeSpan limit = now - newestDate;
                if (limit.TotalDays < 3)
                {
                    throw new Exception($"You have already {userReported.UserName} this place, please wait {3 - limit.Days} days.");
                }
            }

            var reportEntity = new UserReport()
            {
                UserReportId = xserReportId,
                ReportDate = DateTime.Now,
                UserId = report.UserId,
                Content = report.Content,
                Status = "Pending",
            };
            await _unitOfWork.RepositoryUserReport.Insert(reportEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserReport>(reportEntity);
        }

        public async Task<bool> UpdateReport(UserReport report)
            {
                if (report == null) throw new ArgumentNullException(nameof(report));

                // Lấy report từ cơ sở dữ liệu
                var reportEntity = await _unitOfWork.RepositoryUserReport.GetById(report.Id);
                if (reportEntity == null) return false;

                // Chỉ cập nhật trường Status
                reportEntity.Status = report.Status;

                // Cập nhật report trong cơ sở dữ liệu
                _unitOfWork.RepositoryUserReport.Update(reportEntity);
                await _unitOfWork.CommitAsync();

                return true;
            }


        public async Task<ServiceResponseModel<bool>> DeleteReport(int reportId)
        {
            var reportEntity = await _unitOfWork.RepositoryUserReport.GetById(reportId);
            if (reportEntity == null)
            {
                return new ServiceResponseModel<bool>(false);
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
