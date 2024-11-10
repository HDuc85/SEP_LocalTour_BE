using FirebaseAdmin.Messaging;
using Quartz;

namespace LocalTour.Services.Extensions;

public class NotificaitonJob : IJob
{
    public NotificaitonJob()
    {
        
    }
    public async Task Execute(IJobExecutionContext context)
    {
        var deviceTokens = context.MergedJobDataMap.Get("deviceTokens") as List<string>;
        var title = context.MergedJobDataMap.GetString("title");
        var body = context.MergedJobDataMap.GetString("body");
        var message = new MulticastMessage()
        {
            Tokens = deviceTokens,
            Notification = new Notification { Title = title, Body = body }
        };
        await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
    }
}