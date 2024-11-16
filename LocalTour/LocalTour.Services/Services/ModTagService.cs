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

        public async Task<IEnumerable<GetModTagRequest>> GetAllAsync()
        {
            // Fetch all ModTag entries and include Tag details for TagName
            var modTags = await _unitOfWork.RepositoryModTag
                .GetAll()
                .Include(mt => mt.Tag)
                .ToListAsync();

            // Group by UserId and collect detailed Tag information for each UserId
            var userTagGroups = modTags
                .GroupBy(mt => mt.UserId)
                .Select(group => new GetModTagRequest
                {
                    UserId = group.Key,
                    Tags = group.Select(mt => new TagRequest
                    {
                        TagId = mt.TagId,
                        TagName = mt.Tag.TagName
                    }).ToList()
                });

            return userTagGroups;
        }

        public async Task<List<ModTagRequest>> CreateMultipleAsync(ModTagRequest request)
        {
            var userExists = await _unitOfWork.RepositoryUser.GetById(request.UserId);
            if (userExists == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            var modTags = request.TagIds.Select(tagId => new ModTag
            {
                UserId = request.UserId,
                TagId = tagId
            }).ToList();

            foreach (var modTag in modTags)
            {
                await _unitOfWork.RepositoryModTag.Insert(modTag);
            }

            await _unitOfWork.CommitAsync();

            var modTagRequests = modTags.Select(modTag => new ModTagRequest
            {
                UserId = modTag.UserId,
                TagIds = new List<int> { modTag.TagId }
            }).ToList();

            return modTagRequests;
        }

        public async Task<GetModTagRequest> GetTagsByUserAsync(Guid userId)
        {
            // Fetch all tags associated with the user, including the tag name
            var tagRequest = await _unitOfWork.RepositoryModTag
                .GetAll()
                .Where(mt => mt.UserId == userId)
                .Select(mt => new TagRequest
                {
                    TagId = mt.Tag.Id,
                    TagName = mt.Tag.TagName
                })
                .ToListAsync();

            return new GetModTagRequest
            {
                UserId = userId,
                Tags = tagRequest
            };
        }

        public async Task<IEnumerable<GetModTagRequest>> GetUsersByTagAsync(int tagId)
        {
            // Fetch all ModTag entries associated with the specified TagId and include Tag details
            var modTags = await _unitOfWork.RepositoryModTag
                .GetAll()
                .Where(mt => mt.TagId == tagId)
                .Include(mt => mt.Tag)
                .ToListAsync();

            // Group by UserId and collect Tag information for each UserId
            var modTagRequests = modTags
                .GroupBy(mt => mt.UserId)
                .Select(group => new GetModTagRequest
                {
                    UserId = group.Key,
                    Tags = group.Select(mt => new TagRequest
                    {
                        TagId = mt.TagId,
                        TagName = mt.Tag.TagName
                    }).ToList()
                });

            return modTagRequests;
        }

        public async Task<bool> UpdateUserTagsAsync(Guid userId, List<int> tagIds)
        {
            // Retrieve the existing ModTags for the user
            var existingTags = await _unitOfWork.RepositoryModTag
                .GetAll()
                .Where(mt => mt.UserId == userId)
                .ToListAsync();

            // Remove old tags
            foreach (var modTag in existingTags)
            {
                _unitOfWork.RepositoryModTag.Delete(modTag);
            }

            // Add new tags
            var newModTags = tagIds.Select(tagId => new ModTag { UserId = userId, TagId = tagId }).ToList();
            await _unitOfWork.RepositoryModTag.Insert(newModTags);

            // Commit changes
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteMultipleAsync(Guid userId, List<int> tagIds)
        {
            var modTags = await _unitOfWork.RepositoryModTag
                .GetAll()
                .Where(mt => mt.UserId == userId && tagIds.Contains(mt.TagId))
                .ToListAsync();

            foreach (var modTag in modTags)
            {
                _unitOfWork.RepositoryModTag.Delete(modTag);
            }

            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
