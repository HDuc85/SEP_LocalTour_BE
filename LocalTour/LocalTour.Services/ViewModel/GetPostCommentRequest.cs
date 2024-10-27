using LocalTour.Services.ViewModel;

namespace LocalTour.Services.ViewModel
{
    public class GetPostCommentRequest : PaginatedQueryParams
    {
        public int PostId { get; set; }
        public int? ParentId { get; set; } // Optional for fetching child comments
    }
}
