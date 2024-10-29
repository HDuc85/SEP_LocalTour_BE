using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;

namespace LocalTour.Services.Services;

public class FollowUserService : IFollowUserService
{
    private IUnitOfWork _unitOfWork;
    private IUserService _userService;

    public FollowUserService(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<List<User>> GetListUserFollow(Guid userId)
    {
        var userFollowUsers = await _unitOfWork.RepositoryFollowUser.GetData(x => x.UserId == userId);
        var users = await _unitOfWork.RepositoryUser.GetData();

        var results = userFollowUsers.Select(x => users.Single(y => y.Id == x.UserId)).ToList();
        return results;
    }

    public async Task<bool> AddFollowUser(Guid userFollowedId, string phoneNumber)
    {
        var user = await _userService.FindByPhoneNumber(phoneNumber);
        if (user == null)
        {
            return false;
        }

        var check = await _unitOfWork.RepositoryFollowUser.GetData(x =>
            x.UserId == userFollowedId && x.UserFollow == user.Id);
        if (check.Any())
        {
            await _unitOfWork.RepositoryFollowUser.Insert(new FollowUser
            {
                UserFollow = user.Id,
                UserId = userFollowedId,
                DateCreated = DateTime.Now,
            });
            await _unitOfWork.CommitAsync();
            return true; 
        }
        return false;
    }

    public async Task<bool> RemoveFollowUser(Guid userFollowedId, string phoneNumber)
    {
        var user = await _userService.FindByPhoneNumber(phoneNumber);
        if (user == null)
        {
            return false;
        }

        var check = await _unitOfWork.RepositoryFollowUser.GetData(x =>
            x.UserId == userFollowedId &&
            x.UserFollow == user.Id);
        if (check.Any())
        {
            _unitOfWork.RepositoryFollowUser.Delete(check.FirstOrDefault());
            await _unitOfWork.CommitAsync();
            return true; 
        }
        return false;
    }
}