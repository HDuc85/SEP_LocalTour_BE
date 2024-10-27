using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPostCommentService
    {
        Task<PostCommentRequest> CreateCommentAsync(PostCommentRequest request);
        Task<PostCommentRequest?> GetCommentByIdAsync(int id, Guid userId);
        Task<PostCommentRequest?> UpdateCommentAsync(int id, PostCommentRequest request);
        Task<bool> DeleteCommentAsync(int id);
        Task<List<PostCommentRequest>> GetCommentsByPostIdAsync(int postId, Guid userId);
        Task<List<PostMediumRequest>> GetAllMediaByPostId(int postId, PaginatedQueryParams queryParams);
        
    }
}
