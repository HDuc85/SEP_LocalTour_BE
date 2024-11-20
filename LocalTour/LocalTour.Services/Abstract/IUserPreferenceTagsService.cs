using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalTour.Domain.Entities;

namespace LocalTour.Services.Abstract
{
    public interface IUserPreferenceTagsService
    {
        Task<List<Tag>> GetAllUserPreferenceTagsGroupedByUserAsync(string userId);
        Task<UserPreferenceTagsRequest?> GetUserPreferenceTagsById(int id);
        Task<UserPreferenceTagsRequest> CreateUserPreferenceTags(string UserId,UserPreferenceTagsRequest request);
        Task<bool> UpdateUserTagsAsync(Guid userId, List<int> tagIds);
        Task<bool> DeleteUserPreferenceTags(Guid userId, List<int> tagIds);
    }
}
