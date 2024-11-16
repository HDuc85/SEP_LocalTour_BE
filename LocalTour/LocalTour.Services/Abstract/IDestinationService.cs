using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract
{
    public interface IDestinationService
    {
        Task<List<DestinationRequest>> GetAllDestinations(string ?languageCode);
        Task<List<DestinationRequest>> GetAllDestinationsByScheduleId(int scheduleId, string? languageCode);
        Task<DestinationRequest> GetDestinationById(int id, string? languageCode);
        Task<Destination> CreateDestinationAsync(CreateDestinationRequest request);
        Task<bool> UpdateDestinationAsync(int id, CreateDestinationRequest request);
        Task<bool> DeleteDestinationAsync(int id);
    }
}
