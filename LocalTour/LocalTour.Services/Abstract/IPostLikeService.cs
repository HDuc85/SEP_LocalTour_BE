using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPostLikeService
    {
        Task<bool> LikePostAsync(int postId, Guid userId);
        Task<bool> UnlikePostAsync(int postId, Guid userId);
        Task<List<Guid>> GetUserLikesByPostIdAsync(int postId);
        Task<int> GetTotalLikesByPostIdAsync(int postId);
    }
}
