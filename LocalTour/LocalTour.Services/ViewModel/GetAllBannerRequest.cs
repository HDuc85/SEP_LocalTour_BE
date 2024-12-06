namespace LocalTour.Services.ViewModel;

public class GetAllBannerRequest
{
   public String? UserId { get; set; }
   public String? UserName { get; set; }
   public String? BannerName { get; set; }
   public DateTime? DateStart { get; set; }
   public DateTime? DateEnd { get; set; }
   
}