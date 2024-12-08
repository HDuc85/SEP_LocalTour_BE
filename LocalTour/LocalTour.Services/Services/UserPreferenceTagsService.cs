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

        public async Task<List<Tag>> GetAllUserPreferenceTagsGroupedByUserAsync(string userId)
        {
            var user = await _unitOfWork.RepositoryUser.GetById(Guid.Parse(userId));
            // Fetch all ModTag entries and include Tag details for TagName
            var userTags = await _unitOfWork.RepositoryUserPreferenceTags
                .GetAll()
                .Where(x => x.UserId == user.Id )
                .Include(mt => mt.Tag)
                .Select(mt => new Tag
                {
                    Id = mt.TagId,
                    TagName = mt.Tag.TagName,
                    TagVi = mt.Tag.TagVi,
                    TagPhotoUrl = mt.Tag.TagPhotoUrl,
                })
                .ToListAsync();
            

            return userTags;
        }


        public async Task<UserPreferenceTagsRequest?> GetUserPreferenceTagsById(int id)
        {
            var entity = await _unitOfWork.RepositoryUserPreferenceTags.GetById(id);
            return entity == null ? null : _mapper.Map<UserPreferenceTagsRequest>(entity);
        }

        public async Task<UserPreferenceTagsRequest> CreateUserPreferenceTags(string UserId,UserPreferenceTagsRequest request)
        {
            // Check if the user exists
            var userExists = await _unitOfWork.RepositoryUser.GetById(Guid.Parse(UserId));

            if (userExists == null)
            {
                // Handle case when user doesn't exist (optional)
                throw new InvalidOperationException("User does not exist.");
            }

            // Check if all TagIds exist in the Tag table
            
            var userExitsTags = await _unitOfWork.RepositoryUserPreferenceTags.GetAll()
                .Where(x => x.UserId == userExists.Id)
                .Select(y => y.TagId).ToListAsync();
            var existingTagIds = await _unitOfWork.RepositoryTag
                .GetAll()
                .Where(tag => request.TagIds.Contains(tag.Id) && !userExitsTags.Contains(tag.Id))
                .Select(tag => tag.Id)
                .ToListAsync();
            // Create the list of UserPreferenceTags based on the tag IDs provided
            var userPreferenceTags = existingTagIds.Select(tagId => new UserPreferenceTags
            {
                UserId = userExists.Id,
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
            await _unitOfWork.CommitAsync();

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
