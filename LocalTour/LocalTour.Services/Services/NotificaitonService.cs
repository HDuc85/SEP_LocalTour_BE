using FirebaseAdmin.Messaging;
using LocalTour.Data.Abstract;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LocalTour.Services.Services;

public class NotificaitonService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScheduler _scheduler;
    public NotificaitonService(IUnitOfWork unitOfWork, IScheduler scheduler)
    {
        _unitOfWork = unitOfWork;
        _scheduler = scheduler;
    }
    
    public async Task<List<Domain.Entities.Notification>> GetAll(string userId)
    {
        var notifications = _unitOfWork.RepositoryUserNotification.GetDataQueryable(x => x.UserId == Guid.Parse(userId))
            .Include(y => y.Notification).OrderByDescending(z => z.Notification.TimeSend);

       var result = new List<Domain.Entities.Notification>();
       foreach (var notification in notifications)
       {
           result.Add(new Domain.Entities.Notification()
           {
               Id = notification.NotificationId,
               Message = notification.Notification.Message,
               DateCreated = notification.Notification.DateCreated,
               NotificationType = notification.Notification.NotificationType,
               Title = notification.Notification.Title,
               TimeSend = notification.Notification.TimeSend,
               UserNotifications = notification.Notification.UserNotifications
           });
       }
       return result.ToList();
    }
    public async Task<bool> ReadedNotification(string userId, int notificationId)
    {
        var notif = await _unitOfWork.RepositoryUserNotification.GetData(x => x.UserId == Guid.Parse(userId) && x.NotificationId == notificationId);
        notif.First().IsReaded = true;
        _unitOfWork.RepositoryUserNotification.Update(notif.First());
        try
        {
            await _unitOfWork.CommitAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    public async Task<bool> DeleteNotification(int notificationId, string userId)
    {
        var notifcaiton = await _unitOfWork.RepositoryNotification.GetById(notificationId);
        if (notifcaiton.UserId != Guid.Parse(userId))
        {
            return false;
        }
        
        _unitOfWork.RepositoryNotification.Delete(notifcaiton);
        _unitOfWork.RepositoryUserNotification.Delete(x => x.NotificationId == notificationId);
        await _unitOfWork.CommitAsync();

        try
        {
            var jobKey = new JobKey(notificationId.ToString());
            bool deleted = await _scheduler.DeleteJob(jobKey);
            return deleted;
        }catch (Exception ex)
        {
            return false; 
        }
    }
    public async Task<string> SetNotificationForEvent(string userId, int eventId, string title, string body, DateTime timeSend)
    {
        var envent = await _unitOfWork.RepositoryEvent.GetById(eventId);
        if (envent == null)
        {
            return "Event not found";
        }
        var users = await _unitOfWork.RepositoryMarkPlace.GetData(x => x.PlaceId == envent.PlaceId);
        if (users.Any())
        {
            var devices = await _unitOfWork.RepositoryUserDevice.GetData();
            
            var notification = new Domain.Entities.Notification
            {
                Title = title,
                UserId = Guid.Parse(userId),
                NotificationType = "Event",
                TimeSend = timeSend,
                Message = body,
                DateCreated = DateTime.Now,
            };
            await _unitOfWork.RepositoryNotification.Insert(notification);
            await _unitOfWork.CommitAsync();
            var userNotifications = new List<Domain.Entities.UserNotification>();
            foreach (var device in devices)
            {
                userNotifications.Add(new Domain.Entities.UserNotification
                {
                    NotificationId = notification.Id,
                    UserId = device.UserId,
                    IsReaded = false,
                });
            }
            var result = await ScheduleNotification(devices.Select(x => x.DeviceId).ToList(),notification.Id,  title, body, timeSend);

            await _unitOfWork.RepositoryUserNotification.Insert(userNotifications);
            await _unitOfWork.CommitAsync();
            return result;
        }

        return string.Empty;
    }
    public async Task<string> SetNotificationForSystem(string userId ,string title, string body, DateTime timeSend)
    {
        var devices = _unitOfWork.RepositoryUserDevice.GetDataQueryable();
        var notification = new Domain.Entities.Notification
        {
            Title = title,
            UserId = Guid.Parse(userId),
            NotificationType = "System",
            TimeSend = timeSend,
            Message = body,
            DateCreated = DateTime.Now,
        };
        await _unitOfWork.RepositoryNotification.Insert(notification);
        await _unitOfWork.CommitAsync();
        var userNotifications = new List<Domain.Entities.UserNotification>();
        foreach (var device in devices)
        {
            userNotifications.Add(new Domain.Entities.UserNotification
            {
                NotificationId = notification.Id,
                UserId = device.UserId,
                IsReaded = false,
            });
        }
        var result = await ScheduleNotification(devices.Select(x => x.DeviceId).ToList(),notification.Id, title, body, timeSend);

        await _unitOfWork.RepositoryUserNotification.Insert(userNotifications);
        await _unitOfWork.CommitAsync();
        return result;
    }
    public async Task<string> ScheduleNotification(List<string> deviceTokens,int NotificationId, string title, string body, DateTime scheduleTime)
    {
        var jobData = new JobDataMap
        {
            { "deviceTokens", deviceTokens },
            { "title", title },
            { "body", body }
        };
        var job = JobBuilder.Create<NotificaitonJob>()
            .UsingJobData(jobData)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{NotificationId}")
            .StartAt(scheduleTime)
            .Build();
        try
        {
            await _scheduler.ScheduleJob(job, trigger);
            return "Success";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }    }
    public async Task<string> SendNotificationNow(string userId, string deviceToken, string title, string body, string notificationType)
    {
        var usertoken = await _unitOfWork.RepositoryUserDevice.GetData(x => x.DeviceId == deviceToken);
        if (usertoken == null)
        {
            return "Device not found";
        }
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
        var messId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        var notification = new Domain.Entities.Notification()
        {
            Title = title,
            UserId = Guid.Parse(userId),
            NotificationType = notificationType,
            TimeSend = DateTime.Now,
            DateCreated = DateTime.Now,
            Message = body,
        };
        await _unitOfWork.RepositoryNotification.Insert(notification);
        await _unitOfWork.CommitAsync();

        await _unitOfWork.RepositoryUserNotification.Insert(new Domain.Entities.UserNotification()
        {
            NotificationId = notification.Id,
            UserId = usertoken.First().UserId,
        });
        await _unitOfWork.CommitAsync();
        
        if (!string.IsNullOrEmpty(messId))
        {
            return messId;
        }
        return string.Empty;
    }
}