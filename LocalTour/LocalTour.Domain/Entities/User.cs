using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class User : IdentityUser<Guid>
{
    public string? FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public virtual ICollection<FollowUser> FollowUserUserFollowNavigations { get; set; } = new List<FollowUser>();
    public virtual ICollection<FollowUser> FollowUserUsers { get; set; } = new List<FollowUser>();
    public virtual ICollection<MarkPlace> MarkPlaces { get; set; } = new List<MarkPlace>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<PlaceFeeedbackHelpful> PlaceFeeedbackHelpfuls { get; set; } = new List<PlaceFeeedbackHelpful>();
    public virtual ICollection<PlaceFeeedback> PlaceFeeedbacks { get; set; } = new List<PlaceFeeedback>();
    public virtual ICollection<PlaceSearchHistory> PlaceSearchHistories { get; set; } = new List<PlaceSearchHistory>();
    public virtual ICollection<PostCommentLike> PostCommentLikes { get; set; } = new List<PostCommentLike>();
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<ScheduleLike> ScheduleLikes { get; set; } = new List<ScheduleLike>();
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public virtual ICollection<TraveledPlace> TraveledPlaces { get; set; } = new List<TraveledPlace>();
    public virtual ICollection<UserBan> UserBans { get; set; } = new List<UserBan>();
    public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();
    public virtual ICollection<UserReport> UserReports { get; set; } = new List<UserReport>();
    public virtual ICollection<UserReport> UserReporteds { get; set; } = new List<UserReport>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    [JsonIgnore]
    public virtual ICollection<PlaceReport> UserReportPlaces { get; set; } = new List<PlaceReport>();
    public virtual ICollection<UserPreferenceTags> UserPreferenceTags { get; set; } = new List<UserPreferenceTags>();
    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();
    public virtual ICollection<BannerHistory> ApprovedBannerHistories { get; set; } = new List<BannerHistory>();
    
}
