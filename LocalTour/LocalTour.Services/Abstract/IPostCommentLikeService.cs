using System;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPostCommentLikeService
    {
        Task<bool> LikeCommentAsync(int commentId, Guid userId);
        Task<bool> UnlikeCommentAsync(int commentId, Guid userId);
        Task<List<Guid>> GetUserLikesByCommentIdAsync(int postCommentId);
        Task<int> GetTotalLikesByCommentIdAsync(int postCommentId);
    }
}
