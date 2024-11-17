using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface ITagService
    {
        Task<PaginatedList<TagViewModel>> GetAllTag( GetEventRequest request);
        Task<Tag> GetTagById( int tagid);
        Task<TagRequest> CreateTag(TagRequest request);
        Task<TagRequest> UpdateTag(int tagid, TagRequest request);
        Task<bool> DeleteTag( int tagid);
    }
}
