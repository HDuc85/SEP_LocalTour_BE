using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace LocalTour.Services.Services
{
    public class PlaceReportService : IPlaceReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlaceReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PlaceReport> CreateReport(PlaceReportRequest request, String userId)
        {
            var existPlaceReport = await _unitOfWork.RepositoryPlaceReport.GetData(x => x.PlaceId == request.PlaceId && x.UserReportId == Guid.Parse(userId));
            if (existPlaceReport.Any())
            {
                DateTime newestDate = existPlaceReport.Max(item => item.ReportDate);
                DateTime now = DateTime.Now;
                TimeSpan limit = now - newestDate;
                if (limit.TotalDays < 3)
                {
                    throw new Exception($"You have already reported this place, please wait {3 - limit.Days} days.");
                }
            }
            
            var place = await _unitOfWork.RepositoryPlace.GetData(x => x.Id == request.PlaceId);
            if (!place.Any())
            {
                throw new Exception($"There is no place with id: {request.PlaceId}");
            }
            
            var newReport = new PlaceReport()
            {
                PlaceId = request.PlaceId,
                Content = request.Message,
                UserReportId = Guid.Parse(userId),
                ReportDate = DateTime.Now,
                Status = "Pending"
            };
            await _unitOfWork.RepositoryPlaceReport.Insert(newReport);
            await _unitOfWork.CommitAsync();

            return newReport;
        }

        public async Task<IEnumerable<PlaceReportRequest>> GetAllReports()
        {
            var reports = await _unitOfWork.RepositoryPlaceReport.GetAll()
                .Include(pr => pr.UserReport)
                .Include(pr => pr.Place)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PlaceReportRequest>>(reports);
        }

        public async Task<PlaceReportRequest?> GetPlaceReportById(int id)
        {
            var report = await _unitOfWork.RepositoryPlaceReport.GetById(id);
            return _mapper.Map<PlaceReportRequest>(report);
        }

        // Thêm phương thức để lấy báo cáo theo tag
        public async Task<IEnumerable<PlaceReportRequest>> GetReportsByTag(int tagId)
        {
            var reports = await _unitOfWork.RepositoryPlaceTag.GetAll()
                .Where(pt => pt.TagId == tagId)
                .Include(pt => pt.Place)
                .Include(pt => pt.Place.PlaceReports)
                .Select(pt => pt.Place.PlaceReports)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PlaceReportRequest>>(reports);
        }

        public async Task<PlaceReportRequest?> UpdateReport(int id, PlaceReportRequest request)
        {
            var reportEntity = await _unitOfWork.RepositoryPlaceReport.GetById(id);
            if (reportEntity == null) return null; // If not found, return null

            _mapper.Map(request, reportEntity);
            _unitOfWork.RepositoryPlaceReport.Update(reportEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PlaceReportRequest>(reportEntity); // Return updated report
        }

        public async Task<bool> DeleteReport(int id)
        {
            var reportEntity = await _unitOfWork.RepositoryPlaceReport.GetById(id);
            if (reportEntity == null) return false;

            _unitOfWork.RepositoryPlaceReport.Delete(reportEntity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}