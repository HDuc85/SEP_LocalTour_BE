using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IUserPreferenceTagsService
    {
        Task<IEnumerable<GetUserPreferenceTagsRequest>> GetAllUserPreferenceTagsGroupedByUserAsync();
        Task<UserPreferenceTagsRequest?> GetUserPreferenceTagsById(int id);
        Task<UserPreferenceTagsRequest> CreateUserPreferenceTags(UserPreferenceTagsRequest request);
        Task<bool> UpdateUserTagsAsync(Guid userId, List<int> tagIds);
        Task<bool> DeleteUserPreferenceTags(Guid userId, List<int> tagIds);
    }
}
