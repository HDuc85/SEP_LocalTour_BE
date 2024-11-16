using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IModTagService
    {
        Task<IEnumerable<GetModTagRequest>> GetAllAsync();
        Task<GetModTagRequest> GetTagsByUserAsync(Guid userId);
        Task<IEnumerable<GetModTagRequest>> GetUsersByTagAsync(int tagId);
        Task<List<ModTagRequest>> CreateMultipleAsync(ModTagRequest request);
        Task<bool> UpdateUserTagsAsync(Guid userId, List<int> tagIds);
        Task<bool> DeleteMultipleAsync(Guid userId, List<int> tagIds);
    }
}
