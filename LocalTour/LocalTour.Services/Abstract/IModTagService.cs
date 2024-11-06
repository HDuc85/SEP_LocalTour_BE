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
        Task<IEnumerable<ModTag>> GetAllAsync();
        Task<ModTag?> GetByIdAsync(Guid userId, int tagId);
        Task<IEnumerable<ModTagRequest>> GetTagsByUserAsync(Guid userId);
        Task<IEnumerable<ModTagRequest>> GetUsersByTagAsync(int tagId);
        Task<ModTag> CreateAsync(ModTagRequest request);
        Task<ModTag?> UpdateAsync(Guid userId, int tagId, ModTagRequest request);
        Task<bool> DeleteAsync(Guid userId, int tagId);
    }
}
