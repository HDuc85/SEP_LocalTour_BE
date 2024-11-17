using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<GetUserPreferenceTagsRequest>> GetAllUserPreferenceTagsGroupedByUserAsync()
        {
            // Fetch all ModTag entries and include Tag details for TagName
            var userTags = await _unitOfWork.RepositoryUserPreferenceTags
                .GetAll()
                .Include(mt => mt.Tag)
                .ToListAsync();

            // Group by UserId and collect detailed Tag information for each UserId
            var userTagGroups = userTags
                .GroupBy(mt => mt.UserId)
                .Select(group => new GetUserPreferenceTagsRequest
                {
                    UserId = group.Key,
                    Tags = group.Select(mt => new TagVM
                    {
                        TagUrl = mt.Tag.TagPhotoUrl,
                        TagId = mt.TagId,
                        TagName = mt.Tag.TagName
                    }).ToList()
                });

            return userTagGroups;
        }


        public async Task<UserPreferenceTagsRequest?> GetUserPreferenceTagsById(int id)
        {
            var entity = await _unitOfWork.RepositoryUserPreferenceTags.GetById(id);
            return entity == null ? null : _mapper.Map<UserPreferenceTagsRequest>(entity);
        }

        public async Task<UserPreferenceTagsRequest> CreateUserPreferenceTags(UserPreferenceTagsRequest request)
        {
            // Check if the user exists
            var userExists = await _unitOfWork.RepositoryUser.GetById(request.UserId);

            if (userExists == null)
            {
                // Handle case when user doesn't exist (optional)
                throw new InvalidOperationException("User does not exist.");
            }

            // Check if all TagIds exist in the Tag table
            var existingTagIds = await _unitOfWork.RepositoryTag
                .GetAll()
                .Where(tag => request.TagIds.Contains(tag.Id))
                .Select(tag => tag.Id)
                .ToListAsync();

            var invalidTagIds = request.TagIds.Except(existingTagIds).ToList();

            if (invalidTagIds.Any())
            {
                // Handle case for missing tags (either throw error or log)
                throw new InvalidOperationException($"The following TagIds do not exist: {string.Join(", ", invalidTagIds)}");
            }

            // Create the list of UserPreferenceTags based on the tag IDs provided
            var userPreferenceTags = request.TagIds.Select(tagId => new UserPreferenceTags
            {
                UserId = request.UserId,
                TagId = tagId
            }).ToList();

            // Insert each UserPreferenceTag into the correct repository
            foreach (var userPreferenceTag in userPreferenceTags)
            {
                await _unitOfWork.RepositoryUserPreferenceTags.Insert(userPreferenceTag);
            }

            // Commit the transaction to save the changes
            await _unitOfWork.CommitAsync();

            // Returning a single UserPreferenceTagsRequest (adjust according to your needs)
            var result = new UserPreferenceTagsRequest
            {
                UserId = request.UserId,
                TagIds = request.TagIds
            };

            return result;
        }

        public async Task<bool> UpdateUserTagsAsync(Guid userId, List<int> tagIds)
        {
            var existingTags = await _unitOfWork.RepositoryUserPreferenceTags
                .GetAll()
                .Where(ut => ut.UserId == userId)
                .ToListAsync();

            if (existingTags == null || !existingTags.Any()) return false;

            foreach (var userTag in existingTags)
            {
                _unitOfWork.RepositoryUserPreferenceTags.Delete(userTag);
            }

            var newUserTags = tagIds.Select(tagId => new UserPreferenceTags
            {
                UserId = userId,
                TagId = tagId
            }).ToList();

            await _unitOfWork.RepositoryUserPreferenceTags.Insert(newUserTags);

            await _unitOfWork.CommitAsync();
            return true;
        }


        public async Task<bool> DeleteUserPreferenceTags(Guid userId, List<int> tagIds)
        {
            var userTags = await _unitOfWork.RepositoryUserPreferenceTags
                .GetAll()
                .Where(ut => ut.UserId == userId && tagIds.Contains(ut.TagId))
                .ToListAsync();

            if (!userTags.Any())
            {
                return false;
            }

            foreach (var userTag in userTags)
            {
                _unitOfWork.RepositoryUserPreferenceTags.Delete(userTag);
            }

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


    }
}
