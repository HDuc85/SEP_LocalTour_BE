using FirebaseAdmin.Messaging;
using LocalTour.Data.Abstract;
using LocalTour.Services.Abstract;

namespace LocalTour.Services.Services;

public class NotificaitonService : INotificationService 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public NotificaitonService(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }
    
    public async Task SendNotification(string deviceToken, string title, string body)
    {
        var message = new Message()
        {
            FcmOptions = new FcmOptions(){},
            Token = deviceToken,
            Notification = new Notification()
            {
                Title = title,
                Body = body,
            },
        };
        await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }
}