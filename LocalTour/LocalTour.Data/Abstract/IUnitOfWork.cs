using LocalTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Data.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        Repository<Destination> RepositoryDestination {get;}
        Repository<DistrictNcity> RepositoryDistrictNcity {get;}
        Repository<Event> RepositoryEvent {get;}
        Repository<FollowUser>  RepositoryFollowUser {get;}
        Repository<MarkPlace> RepositoryMarkPlace {get;}
        Repository<Notification>  RepositoryNotification {get;}
        Repository<Place> RepositoryPlace {get;}
        Repository<PlaceActivity>  RepositoryPlaceActivity {get;}
        Repository<PlaceActivityMedium> RepositoryPlaceActivityMedium { get; }
        Repository<PlaceFeeedbackMedium> RepositoryPlaceFeeedbackMedium {get;}
        Repository<PlaceMedium> RepositoryPlaceMedium {get;}
        Repository<PlaceReport> RepositoryPlaceReport {get;}
        Repository<ScheduleLike> RepositoryScheduleLike {get;}
        Repository<PostMedium> RepositoryPostMedium {get;}
        Repository<PlaceActivityTranslation>  RepositoryPlaceActivityTranslation {get;}
        Repository<PlaceFeeedback>  RepositoryPlaceFeeedback {get;}
        Repository<PlaceFeeedbackHelpful> RepositoryPlaceFeeedbackHelpful {get;}
        Repository<PlaceSearchHistory> RepositoryPlaceSearchHistory {get;}
        Repository<PlaceTag>  RepositoryPlaceTag {get;}
        Repository<PlaceTranslation> RepositoryPlaceTranslation {get;}
        Repository<Post> RepositoryPost {get;}
        Repository<PostComment>  RepositoryPostComment {get;}
        Repository<PostCommentLike> RepositoryPostCommentLike {get;}
        Repository<PostLike>  RepositoryPostLike {get;}
        Repository<ProvinceNcity> RepositoryProvinceNcity {get;}
        Repository<ScheduleComment>  RepositoryScheduleComment {get;}
        Repository<ScheduleCommentLike> RepositoryScheduleCommentLike {get;}
        Repository<ScheduleUserLike> RepositoryScheduleUserLike {get;}
        Repository<Tag>  RepositoryTag {get;}
        Repository<TraveledPlace> RepositoryTraveledPlace {get;}
        Repository<Schedule>  RepositorySchedule {get;}
        Repository<UserBan> RepositoryUserBan {get;}
        Repository<UserDevice>  RepositoryUserDevice {get;}
        Repository<Ward> RepositoryWard {get;}
        Repository<User> RepositoryUser {get;}
        Repository<Role> RepositoryRole { get; }
        Task CommitAsync();
    }
}
