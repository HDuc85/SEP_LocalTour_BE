using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DestinationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DestinationRequest?> GetDestinationByIdAsync(int id)
        {
            var destination = await _unitOfWork.RepositoryDestination.GetById(id);
            return destination == null ? null : _mapper.Map<DestinationRequest>(destination);
        }

        public async Task<List<DestinationRequest>> GetDestinationsByScheduleIdAsync(int scheduleId)
        {
            var destinations = await _unitOfWork.RepositoryDestination.GetData(d => d.ScheduleId == scheduleId);


            if (destinations == null || !destinations.Any())
            {
                Console.WriteLine("No destinations found for scheduleId " + scheduleId);
            }


            return _mapper.Map<List<DestinationRequest>>(destinations);
        }


        public async Task<Destination> CreateDestinationAsync(DestinationRequest request)
        {
            var destination = _mapper.Map<Destination>(request);

            // Gán StartDay và EndDay tự động từ giờ hệ thống
            destination.StartDate = DateTime.Now; // Gán thời gian hiện tại
            destination.EndDate = DateTime.Now.AddDays(1); // Ví dụ: Gán thời gian kết thúc là một ngày sau

            await _unitOfWork.RepositoryDestination.Insert(destination);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<Destination>(destination);
        }


        public async Task<bool> UpdateDestinationAsync(int id, DestinationRequest request)
        {
            var destination = await _unitOfWork.RepositoryDestination.GetById(id);
            if (destination == null) return false; 

            _mapper.Map(request, destination); 

            // Gán giá trị StartDay và EndDay tự động từ giờ hệ thống
            destination.StartDate = DateTime.Now; // Gán StartDay là thời gian hiện tại
            destination.EndDate = DateTime.Now.AddDays(1); // Gán EndDay là 1 ngày sau StartDay 

            _unitOfWork.RepositoryDestination.Update(destination);
            await _unitOfWork.CommitAsync();

            return true;
        }


        public async Task<bool> DeleteDestinationAsync(int id)
        {
            var destination = await _unitOfWork.RepositoryDestination.GetById(id);
            if (destination == null) return false;

            _unitOfWork.RepositoryDestination.Delete(destination);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
