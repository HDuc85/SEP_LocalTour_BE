using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class UserPreferenceTagsService : IUserPreferenceTagsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserPreferenceTagsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserPreferenceTagsRequest>> GetAllUserPreferenceTags()
        {
            var entities = _unitOfWork.RepositoryUserPreferenceTags.GetAll();
            return _mapper.Map<List<UserPreferenceTagsRequest>>(entities);
        }

        public async Task<UserPreferenceTagsRequest?> GetUserPreferenceTagsById(int id)
        {
            var entity = await _unitOfWork.RepositoryUserPreferenceTags.GetById(id);
            return entity == null ? null : _mapper.Map<UserPreferenceTagsRequest>(entity);
        }

        public async Task<UserPreferenceTagsRequest> CreateUserPreferenceTags(UserPreferenceTagsRequest request)
        {
            var entity = _mapper.Map<UserPreferenceTags>(request);
            await _unitOfWork.RepositoryUserPreferenceTags.Insert(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserPreferenceTagsRequest>(entity);
        }

        public async Task<UserPreferenceTagsRequest?> UpdateUserPreferenceTags(int id, UserPreferenceTagsRequest request)
        {
            var entity = await _unitOfWork.RepositoryUserPreferenceTags.GetById(id);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            _unitOfWork.RepositoryUserPreferenceTags.Update(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserPreferenceTagsRequest>(entity);
        }

        public async Task<bool> DeleteUserPreferenceTags(int id)
        {
            var entity = await _unitOfWork.RepositoryUserPreferenceTags.GetById(id);
            if (entity == null) return false;

            _unitOfWork.RepositoryUserPreferenceTags.Delete(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
