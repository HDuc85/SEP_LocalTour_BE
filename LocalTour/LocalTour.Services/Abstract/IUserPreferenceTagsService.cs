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
        Task<List<UserPreferenceTagsRequest>> GetAllUserPreferenceTags();
        Task<UserPreferenceTagsRequest?> GetUserPreferenceTagsById(int id);
        Task<UserPreferenceTagsRequest> CreateUserPreferenceTags(UserPreferenceTagsRequest request);
        Task<UserPreferenceTagsRequest?> UpdateUserPreferenceTags(int id, UserPreferenceTagsRequest request);
        Task<bool> DeleteUserPreferenceTags(int id);
    }
}
