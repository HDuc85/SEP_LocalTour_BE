using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPostService
    {
        Task<PaginatedList<PostRequest>> GetAllPosts(GetPostRequest request);
        Task<PostRequest?> GetPostById(int id);
        Task<PostRequest> CreatePost(PostRequest request);
        Task<PostRequest?> UpdatePost(int id, PostRequest request);
        Task<bool> DeletePost(int id);
    }
}
