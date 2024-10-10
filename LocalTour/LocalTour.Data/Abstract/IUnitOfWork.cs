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
        Repository<PlaceActivityPhoto> RepositoryPlaceActivityPhoto {get;}
        Repository<PlaceActivityTranslation>  RepositoryPlaceActivityTranslation {get;}
        Repository<PlaceActivityVideo> RepositoryPlaceActivityVideo {get;}
        Repository<PlaceFeeedback>  RepositoryPlaceFeeedback {get;}
        Repository<PlaceFeeedbackHelpful> RepositoryPlaceFeeedbackHelpful {get;}
        Repository<PlaceFeeedbackPhoto>  RepositoryPlaceFeeedbackPhoto {get;}
        Repository<PlaceFeeedbackVideo> RepositoryPlaceFeeedbackVideo {get;}
        Repository<PlacePhoto>  RepositoryPlacePhoto {get;}
        Repository<PlaceSearchHistory> RepositoryPlaceSearchHistory {get;}
        Repository<PlaceTag>  RepositoryPlaceTag {get;}
        Repository<PlaceTranslation> RepositoryPlaceTranslation {get;}
        Repository<PlaceVideo>  RepositoryPlaceVideo {get;}
        Repository<Post> RepositoryPost {get;}
        Repository<PostComment>  RepositoryPostComment {get;}
        Repository<PostCommentLike> RepositoryPostCommentLike {get;}
        Repository<PostLike>  RepositoryPostLike {get;}
        Repository<PostPhoto> RepositoryPostPhoto {get;}
        Repository<PostVideo>  RepositoryPostVideo {get;}
        Repository<ProvinceNcity> RepositoryProvinceNcity {get;}
        Repository<ScheduleComment>  RepositoryScheduleComment {get;}
        Repository<ScheduleCommentLike> RepositoryScheduleCommentLike {get;}
        Repository<ScheduleCommentPhoto>  RepositoryScheduleCommentPhoto {get;}
        Repository<ScheduleCommentVideo> RepositoryScheduleCommentVideo {get;}
        Repository<ScheduleLike>  RepositoryScheduleLike {get;}
        Repository<ScheduleUserLike> RepositoryScheduleUserLike {get;}
        Repository<Tag>  RepositoryTag {get;}
        Repository<TraveledPlace> RepositoryTraveledPlace {get;}
        Repository<Schedule>  RepositorySchedule {get;}
        Repository<UserBan> RepositoryUserBan {get;}
        Repository<UserDevice>  RepositoryUserDevice {get;}
        Repository<Ward> RepositoryWard {get;}
        Repository<Weather>  RepositoryWeather {get;}
        Repository<User> RepositoryUser {get;}
        Repository<Role> RepositoryRole { get; }
        Task CommitAsync();
    }
}
