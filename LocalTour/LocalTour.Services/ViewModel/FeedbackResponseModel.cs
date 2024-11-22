using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;

namespace LocalTour.Services.ViewModel;

public class FeedbackResponseModel : IMapFrom<PlaceFeeedback>
{
    public int FeedbackId { get; set; }
    public String UserFeedbackId { get; set; }
    public String UserName { get; set; }
    public double Rating { get; set; }
    public int FeedbackHelpfulTotal { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public String Content { get; set; }
    public List<PlaceFeeedbackMedium> PlaceFeeedbackMedias { get; set; }
    public bool isLiked { get; set; }
}