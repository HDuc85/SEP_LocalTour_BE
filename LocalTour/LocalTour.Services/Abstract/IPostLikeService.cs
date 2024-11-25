using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract
{
    public interface IPostLikeService
    {
        Task<bool> ToggleLikePostAsync(int postId, Guid userId);
        Task<bool> UnlikePostAsync(int postId, Guid userId);
        Task<List<UserViewModel>> GetUserLikesByPostIdAsync(int postId);
        Task<int> GetTotalLikesByPostIdAsync(int postId);
    }
}
