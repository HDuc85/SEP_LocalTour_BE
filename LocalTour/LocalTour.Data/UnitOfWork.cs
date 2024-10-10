using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        LocalTourDbContext _db;

        private bool disposedValue;

        Repository<Destination> _repositoryDestination;
        Repository<DistrictNcity> _repositoryDistrictNcity;
        Repository<Event> _repositoryEvent;
        Repository<FollowUser> _repositoryFollowUser;
        Repository<MarkPlace> _repositoryMarkPlace;
        Repository<Notification> _repositoryNotification;
        Repository<Place> _repositoryPlace;
        Repository<PlaceActivity> _repositoryPlaceActivity;
        Repository<PlaceActivityPhoto> _repositoryPlaceActivityPhoto;
        Repository<PlaceActivityTranslation> _repositoryPlaceActivityTranslation;
        Repository<PlaceActivityVideo> _repositoryPlaceActivityVideo;
        Repository<PlaceFeeedback> _repositoryPlaceFeeedback;
        Repository<PlaceFeeedbackHelpful> _repositoryPlaceFeeedbackHelpful;
        Repository<PlaceFeeedbackPhoto> _repositoryPlaceFeeedbackPhoto;
        Repository<PlaceFeeedbackVideo> _repositoryPlaceFeeedbackVideo;
        Repository<PlacePhoto> _repositoryPlacePhoto;
        Repository<PlaceSearchHistory> _repositoryPlaceSearchHistory;
        Repository<PlaceTag> _repositoryPlaceTag;
        Repository<PlaceTranslation> _repositoryPlaceTranslation;
        Repository<PlaceVideo> _repositoryPlaceVideo;
        Repository<Post> _repositoryPost;
        Repository<PostComment> _repositoryPostComment;
        Repository<PostCommentLike> _repositoryPostCommentLike;
        Repository<PostLike> _repositoryPostLike;
        Repository<PostPhoto> _repositoryPostPhoto;
        Repository<PostVideo> _repositoryPostVideo;
        Repository<ProvinceNcity> _repositoryProvinceNcity;
        Repository<ScheduleComment> _repositoryScheduleComment;
        Repository<ScheduleCommentLike> _repositoryScheduleCommentLike;
        Repository<ScheduleCommentPhoto> _repositoryScheduleCommentPhoto;
        Repository<ScheduleCommentVideo> _repositoryScheduleCommentVideo;
        Repository<ScheduleLike> _repositoryScheduleLike;
        Repository<ScheduleUserLike> _repositoryScheduleUserLike;
        Repository<Tag> _repositoryTag;
        Repository<TraveledPlace> _repositoryTraveledPlace;
        Repository<Schedule> _repositorySchedule;
        Repository<UserBan> _repositoryUserBan;
        Repository<UserDevice> _repositoryUserDevice;
        Repository<Ward> _repositoryWard;
        Repository<Weather> _repositoryWeather;
        Repository<User> _repositoryUser;
        Repository<Role> _repositoryRole;
        public UnitOfWork(LocalTourDbContext db)
        {
            _db = db;
        }

        public Repository<Destination> RepositoryDestination
        {
        get {
            return _repositoryDestination ??= new Repository<Destination>(_db);
            }
        }
        public Repository<DistrictNcity> RepositoryDistrictNcity {
        get {
            return _repositoryDistrictNcity ??= new Repository<DistrictNcity>(_db);    
            }
        }
        public Repository<Event> RepositoryEvent {
        get {
            return _repositoryEvent ??= new Repository<Event>(_db);    
            }
        }
        public Repository<FollowUser> RepositoryFollowUser {
        get {
            return _repositoryFollowUser ??= new Repository<FollowUser>(_db);    
            }
        }
        public Repository<MarkPlace> RepositoryMarkPlace {
        get {
            return _repositoryMarkPlace ??= new Repository<MarkPlace>(_db);    
            }
        }
        public Repository<Notification> RepositoryNotification {
        get {
            return _repositoryNotification ??= new Repository<Notification>(_db);    
            }
        }
        public Repository<Place> RepositoryPlace {
        get {
            return _repositoryPlace ??= new Repository<Place>(_db);    
            }
        }
        public Repository<PlaceActivity> RepositoryPlaceActivity {
        get {
            return _repositoryPlaceActivity ??= new Repository<PlaceActivity>(_db);    
            }
        }
        public Repository<PlaceActivityPhoto> RepositoryPlaceActivityPhoto {
        get {
            return _repositoryPlaceActivityPhoto ??= new Repository<PlaceActivityPhoto>(_db);    
            }
        }
        public Repository<PlaceActivityTranslation> RepositoryPlaceActivityTranslation {
        get {
            return _repositoryPlaceActivityTranslation ??= new Repository<PlaceActivityTranslation>(_db);    
            }
        }
        public Repository<PlaceActivityVideo> RepositoryPlaceActivityVideo {
        get {
            return _repositoryPlaceActivityVideo ??= new Repository<PlaceActivityVideo>(_db);    
            }
        }
        public Repository<PlaceFeeedback> RepositoryPlaceFeeedback {
        get {
            return _repositoryPlaceFeeedback ??= new Repository<PlaceFeeedback>(_db);    
            }
        }
        public Repository<PlaceFeeedbackHelpful> RepositoryPlaceFeeedbackHelpful {
        get {
            return _repositoryPlaceFeeedbackHelpful ??= new Repository<PlaceFeeedbackHelpful>(_db);    
            }
        }
        public Repository<PlaceFeeedbackPhoto> RepositoryPlaceFeeedbackPhoto {
        get {
            return _repositoryPlaceFeeedbackPhoto ??= new Repository<PlaceFeeedbackPhoto>(_db);    
            }
        }
        public Repository<PlaceFeeedbackVideo> RepositoryPlaceFeeedbackVideo {
        get {
            return _repositoryPlaceFeeedbackVideo ??= new Repository<PlaceFeeedbackVideo>(_db);    
            }
        }
        public Repository<PlacePhoto> RepositoryPlacePhoto {
        get {
            return _repositoryPlacePhoto ??= new Repository<PlacePhoto>(_db);    
            }
        }
        public Repository<PlaceSearchHistory> RepositoryPlaceSearchHistory {
        get {
            return _repositoryPlaceSearchHistory ??= new Repository<PlaceSearchHistory>(_db);    
            }
        }
        public Repository<PlaceTag> RepositoryPlaceTag {
        get {
            return _repositoryPlaceTag ??= new Repository<PlaceTag>(_db);    
            }
        }
        public Repository<PlaceTranslation> RepositoryPlaceTranslation {
        get {
            return _repositoryPlaceTranslation ??= new Repository<PlaceTranslation>(_db);    
            }
        }
        public Repository<PlaceVideo> RepositoryPlaceVideo {
        get {
            return _repositoryPlaceVideo ??= new Repository<PlaceVideo>(_db);    
            }
        }
        public Repository<Post> RepositoryPost {
        get {
            return _repositoryPost ??= new Repository<Post>(_db);    
            }
        }
        public Repository<PostComment> RepositoryPostComment {
        get {
            return _repositoryPostComment ??= new Repository<PostComment>(_db);    
            }
        }
        public Repository<PostCommentLike> RepositoryPostCommentLike {
        get {
            return _repositoryPostCommentLike ??= new Repository<PostCommentLike>(_db);    
            }
        }
        public Repository<PostLike> RepositoryPostLike {
        get {
            return _repositoryPostLike ??= new Repository<PostLike>(_db);    
            }
        }
        public Repository<PostPhoto> RepositoryPostPhoto {
        get {
            return _repositoryPostPhoto ??= new Repository<PostPhoto>(_db);    
            }
        }
        public Repository<PostVideo> RepositoryPostVideo {
        get {
            return _repositoryPostVideo ??= new Repository<PostVideo>(_db);    
            }
        }
        public Repository<ProvinceNcity> RepositoryProvinceNcity {
        get {
            return _repositoryProvinceNcity ??= new Repository<ProvinceNcity>(_db);    
            }
        }
        public Repository<ScheduleComment> RepositoryScheduleComment {
        get {
            return _repositoryScheduleComment ??= new Repository<ScheduleComment>(_db);    
            }
        }
        public Repository<ScheduleCommentLike> RepositoryScheduleCommentLike {
        get {
            return _repositoryScheduleCommentLike ??= new Repository<ScheduleCommentLike>(_db);    
            }
        }
        public Repository<ScheduleCommentPhoto> RepositoryScheduleCommentPhoto {
        get {
            return _repositoryScheduleCommentPhoto ??= new Repository<ScheduleCommentPhoto>(_db);    
            }
        }
        public Repository<ScheduleCommentVideo> RepositoryScheduleCommentVideo {
        get {
            return _repositoryScheduleCommentVideo ??= new Repository<ScheduleCommentVideo>(_db);    
            }
        }
        public Repository<ScheduleLike> RepositoryScheduleLike {
        get {
            return _repositoryScheduleLike ??= new Repository<ScheduleLike>(_db);    
            }
        }
        public Repository<ScheduleUserLike> RepositoryScheduleUserLike {
        get {
            return _repositoryScheduleUserLike ??= new Repository<ScheduleUserLike>(_db);    
            }
        }
        public Repository<Tag> RepositoryTag {
        get {
            return _repositoryTag ??= new Repository<Tag>(_db);    
            }
        }
        public Repository<TraveledPlace> RepositoryTraveledPlace {
        get {
            return _repositoryTraveledPlace ??= new Repository<TraveledPlace>(_db);    
            }
        }
        public Repository<Schedule> RepositorySchedule {
        get {
            return _repositorySchedule ??= new Repository<Schedule>(_db);    
            }
        }
        public Repository<UserBan> RepositoryUserBan {
        get {
            return _repositoryUserBan ??= new Repository<UserBan>(_db);    
            }
        }
        public Repository<UserDevice> RepositoryUserDevice {
        get {
            return _repositoryUserDevice ??= new Repository<UserDevice>(_db);    
            }
        }
        public Repository<Ward> RepositoryWard {
        get {
            return _repositoryWard ??= new Repository<Ward>(_db);    
            }
        }
        public Repository<Weather> RepositoryWeather {
        get {
            return _repositoryWeather ??= new Repository<Weather>(_db);    
            }
        }
        public Repository<User> RepositoryUser {
        get {
            return _repositoryUser ??= new Repository<User>(_db);    
            }
        }
        public Repository<Role> RepositoryRole {
        get {
            return _repositoryRole ??= new Repository<Role>(_db);    
            }
        }

        public async Task CommitAsync()
        {
            await _db.SaveChangesAsync();
        }
        public void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing : true);
            GC.SuppressFinalize(this);
        }
    }
}
