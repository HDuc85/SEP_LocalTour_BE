using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Claims;

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

        public async Task<PlaceReportVM> GetPlaceReportById(int id)
        {
            var report = await _unitOfWork.RepositoryPlaceReport.GetDataQueryable()
                .Include(pr => pr.UserReport)
                .Include(pr => pr.Place)
                .Include(pr => pr.Place.PlaceTranslations)
                .FirstOrDefaultAsync(e => e.Id == id);
            return _mapper.Map<PlaceReportVM>(report);
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

        public async Task<PaginatedList<PlaceReportVM>> GetAllPlaceReportByMod(PlaceReportViewModel request,String userId)
        {
           
            var modTags = await _unitOfWork.RepositoryModTag.GetAll()
                .Where(mt => mt.UserId == Guid.Parse(userId))
                .Select(s => s.DistrictNcityId)
                .ToListAsync();

            if (request.DistrictNCityIds != null && request.DistrictNCityIds.Any())
            {
                modTags = modTags.Where(x => request.DistrictNCityIds.Contains(x)).ToList();
            }
            IQueryable<PlaceReport> reports;
                reports = _unitOfWork.RepositoryPlaceReport.GetAll()
                        .Include(y => y.UserReport)
                        .Include(z => z.Place)
                        .Where(x => modTags.Contains(x.Place.Ward.DistrictNcityId))
                        .AsQueryable();

            if (request.Status != null)
            {
                reports = reports.Where(p => p.Status == request.Status);
            }
            if (request.SearchTerm is not null)
            {
                reports = reports.Where(e =>
                        (e.Place.PlaceTranslations != null && e.Place.PlaceTranslations.Any(t => t.Name.Contains(request.SearchTerm))) ||
                        (e.UserReport != null && e.UserReport.UserName.Contains(request.SearchTerm))
);
            }

            return await reports
                .ListPaginateWithSortAsync<PlaceReport, PlaceReportVM>(
                    request.Page,
                    request.Size,
                    request.SortBy,
                    request.SortOrder,
                    _mapper.ConfigurationProvider);
        }
    }
}