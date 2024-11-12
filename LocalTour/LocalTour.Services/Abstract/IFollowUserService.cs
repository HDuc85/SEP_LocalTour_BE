using LocalTour.Domain.Entities;
using LocalTour.Services.Model;

namespace LocalTour.Services.Abstract;

public interface IFollowUserService
{
    Task<List<User>> GetListUserFollow(Guid userId);
    Task<List<User>> GetListUserFollowed(Guid userId);
    Task<ServiceResponseModel<User>> AddFollowUser(Guid userFollowedId, string userId);
    Task<ServiceResponseModel<bool>> RemoveFollowUser(Guid userFollowedId, string userId);
    
}