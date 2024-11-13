using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPostService
    {
        Task<PostRequest> GetPostById(int postId, Guid currentUserId);
        Task<PaginatedList<PostRequest>> GetAllPosts(GetPostRequest request);
        Task<PostRequest> CreatePost(CreatePostRequest createPostRequest);
        Task<PostRequest?> UpdatePost(int postId, CreatePostRequest createPostRequest);
        Task<bool> DeletePost(int id);
        Guid GetCurrentUserId();
    }
}
