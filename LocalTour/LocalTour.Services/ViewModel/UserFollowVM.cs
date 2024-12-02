using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;

namespace LocalTour.Services.ViewModel;

public class UserFollowVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserProfileUrl { get; set; }

}