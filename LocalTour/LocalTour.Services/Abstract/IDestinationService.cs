using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract
{
    public interface IDestinationService
    {
        Task<DestinationRequest?> GetDestinationByIdAsync(int id);
        Task<List<DestinationRequest>> GetDestinationsByScheduleIdAsync(int scheduleId); // Add this line
        Task<Destination> CreateDestinationAsync(DestinationRequest request);
        Task<bool> UpdateDestinationAsync(int id, DestinationRequest request);
        Task<bool> DeleteDestinationAsync(int id);
    }
}
