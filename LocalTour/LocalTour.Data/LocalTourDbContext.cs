using System;
using System.Collections.Generic;
using LocalTour.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LocalTour.Data;

public partial class LocalTourDbContext : IdentityDbContext<User,Role,Guid>
{
    public LocalTourDbContext()
    {
    }

    public LocalTourDbContext(DbContextOptions<LocalTourDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Destination> Destinations { get; set; }
    public virtual DbSet<DistrictNcity> DistrictNcities { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<FollowUser> FollowUsers { get; set; }
    public virtual DbSet<MarkPlace> MarkPlaces { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Place> Places { get; set; }
    public virtual DbSet<PlaceActivity> PlaceActivities { get; set; }
    public virtual DbSet<PlaceActivityPhoto> PlaceActivityPhotos { get; set; }
    public virtual DbSet<PlaceActivityTranslation> PlaceActivityTranslations { get; set; }
    public virtual DbSet<PlaceActivityVideo> PlaceActivityVideos { get; set; }
    public virtual DbSet<PlaceFeeedback> PlaceFeeedbacks { get; set; }
    public virtual DbSet<PlaceFeeedbackHelpful> PlaceFeeedbackHelpfuls { get; set; }
    public virtual DbSet<PlaceFeeedbackPhoto> PlaceFeeedbackPhotos { get; set; }
    public virtual DbSet<PlaceFeeedbackVideo> PlaceFeeedbackVideos { get; set; }
    public virtual DbSet<PlacePhoto> PlacePhotos { get; set; }
    public virtual DbSet<PlaceSearchHistory> PlaceSearchHistories { get; set; }
    public virtual DbSet<PlaceTag> PlaceTags { get; set; }
    public virtual DbSet<PlaceTranslation> PlaceTranslations { get; set; }
    public virtual DbSet<PlaceVideo> PlaceVideos { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<PostComment> PostComments { get; set; }
    public virtual DbSet<PostCommentLike> PostCommentLikes { get; set; }
    public virtual DbSet<PostLike> PostLikes { get; set; }
    public virtual DbSet<PostPhoto> PostPhotos { get; set; }
    public virtual DbSet<PostVideo> PostVideos { get; set; }
    public virtual DbSet<ProvinceNcity> ProvinceNcities { get; set; }
    public virtual DbSet<Schedule> Schedules { get; set; }
    public virtual DbSet<ScheduleComment> ScheduleComments { get; set; }
    public virtual DbSet<ScheduleCommentLike> ScheduleCommentLikes { get; set; }
    public virtual DbSet<ScheduleCommentPhoto> ScheduleCommentPhotos { get; set; }
    public virtual DbSet<ScheduleCommentVideo> ScheduleCommentVideos { get; set; }
    public virtual DbSet<ScheduleLike> ScheduleLikes { get; set; }
    public virtual DbSet<ScheduleUserLike> ScheduleUserLikes { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<TraveledPlace> TraveledPlaces { get; set; }
    public virtual DbSet<UserBan> UserBans { get; set; }
    public virtual DbSet<UserDevice> UserDevices { get; set; }
    public virtual DbSet<Ward> Wards { get; set; }
    public virtual DbSet<Weather> Weathers { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Destinat__3214EC070081D318");

            entity.ToTable("Destination");

            entity.Property(e => e.Detail).HasMaxLength(500);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Place).WithMany(p => p.Destinations)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Destinati__Place__3E1D39E1");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Destinations)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Destinati__Sched__3D2915A8");
        });

        modelBuilder.Entity<DistrictNcity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__District__3214EC07B5C42593");

            entity.ToTable("DistrictNCity");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.ProvinceNcityId).HasColumnName("ProvinceNCityId");

            entity.HasOne(d => d.ProvinceNcity).WithMany(p => p.DistrictNcities)
                .HasForeignKey(d => d.ProvinceNcityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DistrictN__Provi__3B40CD36");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Event__3214EC07AA2AA3BF");

            entity.ToTable("Event");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EventName).HasMaxLength(256);
            entity.Property(e => e.EventStatus).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Place).WithMany(p => p.Events)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Event__PlaceId__4C6B5938");
        });

        modelBuilder.Entity<FollowUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FollowUs__3214EC07AA056334");

            entity.ToTable("FollowUser");

            entity.Property(e => e.DateCreated).HasColumnType("datetime");

            entity.HasOne(d => d.UserFollowNavigation).WithMany(p => p.FollowUserUserFollowNavigations)
                .HasForeignKey(d => d.UserFollow)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FollowUse__UserF__208CD6FA");

            entity.HasOne(d => d.User).WithMany(p => p.FollowUserUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FollowUse__UserI__1F98B2C1");
        });

        modelBuilder.Entity<MarkPlace>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MarkPlac__3214EC071644C9E2");

            entity.ToTable("MarkPlace");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Place).WithMany(p => p.MarkPlaces)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MarkPlace__Place__22751F6C");

            entity.HasOne(d => d.User).WithMany(p => p.MarkPlaces)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MarkPlace__UserI__2180FB33");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0761034B2D");

            entity.ToTable("Notification");

            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.NotificationType).HasMaxLength(50);
            entity.Property(e => e.TimeSend).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__1EA48E88");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Place__3214EC07B8454B46");

            entity.ToTable("Place");

            entity.HasOne(d => d.Ward).WithMany(p => p.Places)
                .HasForeignKey(d => d.WardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Place__WardId__31B762FC");
        });

        modelBuilder.Entity<PlaceActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceAct__3214EC07793FB945");

            entity.ToTable("PlaceActivity");

            entity.HasOne(d => d.Place).WithMany(p => p.PlaceActivities)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceActi__Place__32AB8735");
        });

        modelBuilder.Entity<PlaceActivityPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceAct__3214EC079250A7A9");

            entity.ToTable("PlaceActivityPhoto");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.PlaceActivity).WithMany(p => p.PlaceActivityPhotos)
                .HasForeignKey(d => d.PlaceActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceActi__Place__3493CFA7");
        });

        modelBuilder.Entity<PlaceActivityTranslation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceAct__3214EC0782C5AAD1");

            entity.ToTable("PlaceActivityTranslation");

            entity.Property(e => e.ActivityName).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.Property(e => e.LanguageCode).HasMaxLength(10);
            entity.Property(e => e.PriceType).HasMaxLength(256);

            entity.HasOne(d => d.PlaceActivity).WithMany(p => p.PlaceActivityTranslations)
                .HasForeignKey(d => d.PlaceActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceActi__Place__4B7734FF");
        });

        modelBuilder.Entity<PlaceActivityVideo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceAct__3214EC07228A83C5");

            entity.ToTable("PlaceActivityVideo");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.PlaceActivity).WithMany(p => p.PlaceActivityVideos)
                .HasForeignKey(d => d.PlaceActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceActi__Place__339FAB6E");
        });

        modelBuilder.Entity<PlaceFeeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceFee__3214EC07A9B6DB68");

            entity.ToTable("PlaceFeeedback");

            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Place).WithMany(p => p.PlaceFeeedbacks)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceFeee__Place__18EBB532");

            entity.HasOne(d => d.User).WithMany(p => p.PlaceFeeedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceFeee__UserI__19DFD96B");
        });

        modelBuilder.Entity<PlaceFeeedbackHelpful>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceFee__3214EC0713FD68AF");

            entity.ToTable("PlaceFeeedbackHelpful");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.PlaceFeedBack).WithMany(p => p.PlaceFeeedbackHelpfuls)
                .HasForeignKey(d => d.PlaceFeedBackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceFeee__Place__40F9A68C");

            entity.HasOne(d => d.User).WithMany(p => p.PlaceFeeedbackHelpfuls)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceFeee__UserI__41EDCAC5");
        });

        modelBuilder.Entity<PlaceFeeedbackPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceFee__3214EC074759D9FC");

            entity.ToTable("PlaceFeeedbackPhoto");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Feedback).WithMany(p => p.PlaceFeeedbackPhotos)
                .HasForeignKey(d => d.FeedbackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceFeee__Feedb__1AD3FDA4");
        });

        modelBuilder.Entity<PlaceFeeedbackVideo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceFee__3214EC07C34ADB5E");

            entity.ToTable("PlaceFeeedbackVideo");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.PlaceFeeedbackVideo)
                .HasForeignKey<PlaceFeeedbackVideo>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceFeeedba__Id__1DB06A4F");
        });

        modelBuilder.Entity<PlacePhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlacePho__3214EC073D1D4BD3");

            entity.ToTable("PlacePhoto");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.PlacePhoto)
                .HasForeignKey<PlacePhoto>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlacePhoto__Id__1CBC4616");
        });

        modelBuilder.Entity<PlaceSearchHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceSea__3214EC07DBC9BF2F");

            entity.ToTable("PlaceSearchHistory");

            entity.Property(e => e.LastSearch).HasColumnType("datetime");

            entity.HasOne(d => d.Place).WithMany(p => p.PlaceSearchHistories)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceSear__Place__37703C52");

            entity.HasOne(d => d.User).WithMany(p => p.PlaceSearchHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceSear__UserI__367C1819");
        });

        modelBuilder.Entity<PlaceTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceTag__3214EC07E4D618F7");

            entity.ToTable("PlaceTag");

            entity.HasOne(d => d.Place).WithMany(p => p.PlaceTags)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceTag__PlaceI__282DF8C2");

            entity.HasOne(d => d.Tag).WithMany(p => p.PlaceTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceTag__TagId__29221CFB");
        });

        modelBuilder.Entity<PlaceTranslation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceTra__3214EC0772AACA45");

            entity.ToTable("PlaceTranslation");

            entity.Property(e => e.Address).HasMaxLength(256);
            entity.Property(e => e.Contact).HasMaxLength(256);
            entity.Property(e => e.LanguageCode).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(256);

            entity.HasOne(d => d.Place).WithMany(p => p.PlaceTranslations)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceTran__Place__4A8310C6");
        });

        modelBuilder.Entity<PlaceVideo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlaceVid__3214EC0798D9E211");

            entity.ToTable("PlaceVideo");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Place).WithMany(p => p.PlaceVideos)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaceVide__Place__1BC821DD");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC07914C02B0");

            entity.ToTable("Post");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post__AuthorId__30C33EC3");

            entity.HasOne(d => d.Place).WithMany(p => p.Posts)
                .HasForeignKey(d => d.PlaceId)
                .HasConstraintName("FK__Post__PlaceId__2FCF1A8A");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Posts)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK__Post__ScheduleId__4D5F7D71");
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostComm__3214EC074E38A4E7");

            entity.ToTable("PostComment");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__PostComme__Paren__2DE6D218");

            entity.HasOne(d => d.Post).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostComme__PostI__2EDAF651");

            entity.HasOne(d => d.User).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostComme__UserI__3864608B");
        });

        modelBuilder.Entity<PostCommentLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostComm__3214EC0767DC41D9");

            entity.ToTable("PostCommentLike");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.PostComment).WithMany(p => p.PostCommentLikes)
                .HasForeignKey(d => d.PostCommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostComme__PostC__44CA3770");

            entity.HasOne(d => d.User).WithMany(p => p.PostCommentLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostComme__UserI__45BE5BA9");
        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostLike__3214EC07EC6C5019");

            entity.ToTable("PostLike");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostLike__PostId__42E1EEFE");

            entity.HasOne(d => d.User).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostLike__UserId__43D61337");
        });

        modelBuilder.Entity<PostPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostPhot__3214EC07774C3FF7");

            entity.ToTable("PostPhoto");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.PostPhotos)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostPhoto__PostI__3A4CA8FD");
        });

        modelBuilder.Entity<PostVideo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostVide__3214EC07041166FA");

            entity.ToTable("PostVideo");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.PostVideos)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostVideo__PostI__395884C4");
        });

        modelBuilder.Entity<ProvinceNcity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Province__3214EC07C5725311");

            entity.ToTable("ProvinceNCity");

            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<ScheduleComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC07963FA531");

            entity.ToTable("ScheduleComment");

            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ParentFeedBack).WithMany(p => p.InverseParentFeedBack)
                .HasForeignKey(d => d.ParentFeedBackId)
                .HasConstraintName("FK__ScheduleC__Paren__3587F3E0");

            entity.HasOne(d => d.Schedule).WithMany(p => p.ScheduleComments)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleC__Sched__245D67DE");

            entity.HasOne(d => d.User).WithMany(p => p.ScheduleComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleC__UserI__25518C17");
        });

        modelBuilder.Entity<ScheduleCommentLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC07CE423D8A");

            entity.ToTable("ScheduleCommentLike");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ScheduleComment).WithMany(p => p.ScheduleCommentLikes)
                .HasForeignKey(d => d.ScheduleCommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleC__Sched__47A6A41B");

            entity.HasOne(d => d.User).WithMany(p => p.ScheduleCommentLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleC__UserI__489AC854");
        });

        modelBuilder.Entity<ScheduleCommentPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC0709B13D9B");

            entity.ToTable("ScheduleCommentPhoto");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Feedback).WithMany(p => p.ScheduleCommentPhotos)
                .HasForeignKey(d => d.FeedbackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleC__Feedb__2645B050");
        });

        modelBuilder.Entity<ScheduleCommentVideo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC0786619F4F");

            entity.ToTable("ScheduleCommentVideo");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Feedback).WithMany(p => p.ScheduleCommentVideos)
                .HasForeignKey(d => d.FeedbackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleC__Feedb__2739D489");
        });

        modelBuilder.Entity<ScheduleLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC07E0891146");

            entity.ToTable("ScheduleLike");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Schedule).WithMany(p => p.ScheduleLikes)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleL__Sched__46B27FE2");

            entity.HasOne(d => d.User).WithMany(p => p.ScheduleLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleL__UserI__498EEC8D");
        });

        modelBuilder.Entity<ScheduleUserLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC0775DECF96");

            entity.ToTable("ScheduleUserLike");

            entity.Property(e => e.IsPublic).HasDefaultValue(false);

            entity.HasOne(d => d.Schedule).WithMany(p => p.ScheduleUserLikes)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleU__Sched__3F115E1A");

            entity.HasOne(d => d.User).WithMany(p => p.ScheduleUserLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ScheduleU__UserI__40058253");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tag__3214EC072F2008B6");

            entity.ToTable("Tag");

            entity.Property(e => e.TagName).HasMaxLength(256);
        });

        modelBuilder.Entity<TraveledPlace>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Traveled__3214EC07754EA6A4");

            entity.ToTable("TraveledPlace");

            entity.Property(e => e.TimeArrive).HasColumnType("datetime");

            entity.HasOne(d => d.Place).WithMany(p => p.TraveledPlaces)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TraveledP__Place__2B0A656D");

            entity.HasOne(d => d.User).WithMany(p => p.TraveledPlaces)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TraveledP__UserI__2A164134");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC07641F6D14");

            entity.ToTable("Schedule");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsPublic).HasDefaultValue(false);
            entity.Property(e => e.ScheduleName).HasMaxLength(256);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedule__UserId__236943A5");
        });

        modelBuilder.Entity<UserBan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserBan__3214EC074CE9229C");

            entity.ToTable("UserBan");

            entity.Property(e => e.EndDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserBans)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserBan__UserId__2BFE89A6");
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserDevi__3214EC07004415BB");

            entity.ToTable("UserDevice");

            entity.Property(e => e.DeviceId)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserDevic__UserI__2CF2ADDF");
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ward__3214EC07FC3D4893");

            entity.ToTable("Ward");

            entity.Property(e => e.DistrictNcityId).HasColumnName("DistrictNCityId");
            entity.Property(e => e.WardName).HasMaxLength(256);

            entity.HasOne(d => d.DistrictNcity).WithMany(p => p.Wards)
                .HasForeignKey(d => d.DistrictNcityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ward__DistrictNC__3C34F16F");
        });

        modelBuilder.Entity<Weather>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Weather__3214EC075618987C");

            entity.ToTable("Weather");

            entity.Property(e => e.Location).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.Weathers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Weather__UserId__4E53A1AA");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07709974E8");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(256);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

        });

        modelBuilder.Entity<Role>(e => e.ToTable(name: "Roles"));

        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("RoleClaims");

            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("UserClaims");
            
            entity.HasKey(e => e.Id).HasName("PK__UserClai__3214EC07678B1B3B");
        });

        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("UserLogins");
            entity.HasKey(e => e.LoginProvider);
            entity.HasKey(e => e.ProviderKey);
        });

        modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("UserTokens");

            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK__UserToke__8CC49841B601027B");
        });


        modelBuilder.Entity<IdentityUserRole<Guid>>(entity => { 
            entity.ToTable("UserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
