namespace LocalTour.Services.ViewModel;

public class SuggestScheduleRequest
{
    public double userLongitude {get;set;}    
    public double userLatitude {get;set;}    
    public string languageCode {get;set;}
    public DateTime startDate {get;set;} = DateTime.Today;
    public int? days { get; set; } = 3;
}