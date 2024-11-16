using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPostCommentService
    {
        Task<PostComment> CreateCommentAsync(CreatePostCommentRequest request, Guid parsedUserId);
        Task<List<PostCommentRequest>> GetCommentsByPostIdAsync(int postId, int? parentId, Guid userId);
        Task<PostCommentRequest?> UpdateCommentAsync(int id, UpdatePostCommentRequest request);
        Task<bool> DeleteCommentAsync(int id);
        //Task<List<PostMediumRequest>> GetAllMediaByPostId(int postId, PaginatedQueryParams queryParams);
    }
}
