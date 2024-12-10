using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;

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
        var userFollowUsers = await _unitOfWork.RepositoryFollowUser.GetData(x => x.UserFollow == userId);
        var users = await _unitOfWork.RepositoryUser.GetData();

        var results = userFollowUsers.Select(x => users.Single(y => y.Id == x.UserId)).ToList();
        return results;
    }
    
    public async Task<List<User>> GetListUserFollowed(Guid userId)
    {
        var userFollowUsers = await _unitOfWork.RepositoryFollowUser.GetData(x => x.UserId == userId);
        var users = await _unitOfWork.RepositoryUser.GetData();

        var results = userFollowUsers.Select(x => users.Single(y => y.Id == x.UserFollow)).ToList();
        return results;
    }
    public async Task<ServiceResponseModel<User>> AddFollowUser(Guid userFollowedId, string userId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return new ServiceResponseModel<User>(false, "User does not exist");
        }

        var check = await _unitOfWork.RepositoryFollowUser.GetData(x =>
            x.UserId == userFollowedId && x.UserFollow == user.Id);
        if (!check.Any())
        {
            await _unitOfWork.RepositoryFollowUser.Insert(new FollowUser
            {
                UserFollow = user.Id,
                UserId = userFollowedId,
                DateCreated = DateTime.Now,
            });
            await _unitOfWork.CommitAsync();
            return new ServiceResponseModel<User>(true, "Followed successfully"); 
        }
        return new ServiceResponseModel<User>(false, "User has followed");
    }

    public async Task<ServiceResponseModel<bool>> RemoveFollowUser(Guid userFollowedId, string userId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return new ServiceResponseModel<bool>(false, "User does not exist");
        }

        var check = await _unitOfWork.RepositoryFollowUser.GetData(x =>
            x.UserId == userFollowedId &&
            x.UserFollow == user.Id);
        if (check.Any())
        {
            _unitOfWork.RepositoryFollowUser.Delete(check.FirstOrDefault());
            await _unitOfWork.CommitAsync();
            return new ServiceResponseModel<bool>(true, "Unfollow successfully"); 
        }
        return new ServiceResponseModel<bool>(false, "User has not followed");
    }
}