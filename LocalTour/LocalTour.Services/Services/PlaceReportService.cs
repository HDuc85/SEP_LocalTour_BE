using AutoMapper;
using LocalTour.Data;
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

        public async Task<PlaceReportRequest> CreateReport(PlaceReportRequest request)
        {
            var reportEntity = _mapper.Map<PlaceReport>(request);
            await _unitOfWork.RepositoryPlaceReport.Insert(reportEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PlaceReportRequest>(reportEntity);
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