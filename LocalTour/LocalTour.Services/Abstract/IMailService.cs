using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface IMailService
{
   public void SendEmail(SendEmailModel model);
}