using LocalTour.Domain.Entities;

namespace LocalTour.Services.Abstract;

public interface IFollowUserService
{
    Task<List<User>> GetListUserFollow(Guid userId);
    Task<bool> AddFollowUser(Guid userFollowedId, string phoneNumber);
    Task<bool> RemoveFollowUser(Guid userFollowedId, string phoneNumber);
    
}