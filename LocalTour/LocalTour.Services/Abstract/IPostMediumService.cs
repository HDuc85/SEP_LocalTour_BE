using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract
{
    public interface IPostMediumService
    {
        Task<List<PostMediumRequest>> GetAllMediaByPostId(int postId);
        Task<PostMediumRequest?> GetMediaById(int mediaId);
        Task<PostMediumRequest> CreateMedia(PostMediumRequest request);
        Task<PostMediumRequest?> UpdateMedia(int mediaId, PostMediumRequest request);
        Task<bool> DeleteMedia(int mediaId);
    }
}
