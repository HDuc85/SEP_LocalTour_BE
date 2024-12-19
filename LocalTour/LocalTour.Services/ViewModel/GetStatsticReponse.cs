namespace LocalTour.Services.ViewModel;

public class GetStatsticReponse
{
    public int Month { get; set; }
    public int Total { get; set; }
    public int TotalPrice { get; set; }
}

public class StatsticMonth
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Avatar { get; set; }
    public List<GetStatsticReponse> list { get; set; }
}

public class PlaceStatsticMonth 
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Avatar { get; set; }
    public List<PlaceGetStatsticReponse> list { get; set; }
}

public class PlaceGetStatsticReponse : GetStatsticReponse
{
    public int TotalPlaceUnpaid { get; set; }
    public int TotalPlacePaid { get; set; }
}