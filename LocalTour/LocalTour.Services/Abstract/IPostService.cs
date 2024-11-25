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
        Task<ServiceResponseModel<PostRequest>> GetPostById(int postId, String currentUserId);
        Task<List<PostRequest>> GetAllPosts(GetPostRequest request, String userId);
        Task<PostRequest> CreatePost(CreatePostRequest createPostRequest, String parsedUserId);
        Task<bool> UpdatePost(int postId, CreatePostRequest createPostRequest, String parsedUserId);
        Task<bool> DeletePost(int postId, Guid guid);
        Guid GetCurrentUserId();
        Task<List<PostCommentRequest>> GetCommentsByPostIdAsync(int postId, String userId );
    }
}
