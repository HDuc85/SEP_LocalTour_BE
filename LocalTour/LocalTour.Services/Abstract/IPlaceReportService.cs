﻿using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract
{
    public interface IPlaceReportService
    {
        Task<PlaceReport> CreateReport(PlaceReportRequest request, String UserId);
        Task<IEnumerable<PlaceReportRequest>> GetAllReports();
        Task<PlaceReportRequest?> GetPlaceReportById(int id);
        Task<IEnumerable<PlaceReportRequest>> GetReportsByTag(int tagId);
        Task<PlaceReportRequest?> UpdateReport(int id, PlaceReportRequest request);
        Task<bool> DeleteReport(int id);
    }
}
