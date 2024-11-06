using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class ModTagService : IModTagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ModTagService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ModTag>> GetAllAsync()
        {
            return await _unitOfWork.RepositoryModTag.GetData();
        }

        public async Task<ModTag?> GetByIdAsync(Guid userId, int tagId)
        {
            return await _unitOfWork.RepositoryModTag
                .GetAll() // Assuming GetAll() returns an IQueryable
                .FirstOrDefaultAsync(e => e.UserId == userId && e.TagId == tagId);
        }

        public async Task<ModTag> CreateAsync(ModTagRequest request)
        {
            var modTag = _mapper.Map<ModTag>(request);
            await _unitOfWork.RepositoryModTag.Insert(modTag);
            await _unitOfWork.CommitAsync();
            return modTag;
        }

        // Method to get all tags owned by a specific user
        public async Task<IEnumerable<ModTagRequest>> GetTagsByUserAsync(Guid userId)
        {
            // Use GetAll to fetch all ModTag entries for the specific user
            var modTags = await _unitOfWork.RepositoryModTag
                .GetAll() // Assuming GetAll() returns an IQueryable
                .Where(mt => mt.UserId == userId)
                .ToListAsync();

            // Mapping to ModTagRequest for the response
            return _mapper.Map<IEnumerable<ModTagRequest>>(modTags);
        }

        // Method to get all users associated with a specific tag
        public async Task<IEnumerable<ModTagRequest>> GetUsersByTagAsync(int tagId)
        {
            // Use GetAll to fetch all ModTag entries for the specific tag
            var modTags = await _unitOfWork.RepositoryModTag
                .GetAll() // Assuming GetAll() returns an IQueryable
                .Where(mt => mt.TagId == tagId)
                .ToListAsync();

            // Mapping to ModTagRequest for the response
            return _mapper.Map<IEnumerable<ModTagRequest>>(modTags);
        }

        public async Task<ModTag?> UpdateAsync(Guid userId, int tagId, ModTagRequest request)
        {
            var modTag = await _unitOfWork.RepositoryModTag
                .GetAll()
                .FirstOrDefaultAsync(mt => mt.UserId == userId && mt.TagId == tagId);

            if (modTag == null)
            {
                return null; // Handle the case where the entity is not found
            }

            // Delete the old entity
            _unitOfWork.RepositoryModTag.Delete(modTag);

            var newModTag = new ModTag
            {
                UserId = request.UserId,  // Assuming you have these properties in your request
                TagId = request.TagId,
                // Set other properties accordingly
            };

            // Add the new entity
            await _unitOfWork.RepositoryModTag.Insert(newModTag);
            await _unitOfWork.CommitAsync();


            return newModTag;
        }


        public async Task<bool> DeleteAsync(Guid userId, int tagId)
        {
            var modTag = await GetByIdAsync(userId, tagId);
            if (modTag == null) return false;

            _unitOfWork.RepositoryModTag.Delete(modTag);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
