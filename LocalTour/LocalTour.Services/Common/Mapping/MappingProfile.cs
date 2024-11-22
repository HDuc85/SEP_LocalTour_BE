using System.Reflection;
using AutoMapper;
using Azure.Core;
using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;


namespace Service.Common.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map from Post to PostRequest
        CreateMap<Post, PostRequest>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.PostMedia.Select(pm => new PostMediumRequest
            {
                Id = pm.Id,
                Type = pm.Type,
                Url = pm.Url,
                CreateDate = pm.CreateDate
            }).ToList()))
            .ForMember(dest => dest.AuthorFullName, opt => opt.MapFrom(src => src.Author.FullName))
            .ForMember(dest => dest.AuthorProfilePictureUrl, opt => opt.MapFrom(src => src.Author.ProfilePictureUrl));


        // Map from PostRequest to Post
        CreateMap<PostRequest, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id to allow auto-generation
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set creation time
            .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.UtcNow)); // Set update time

        // Reverse mapping for PostMedium
        CreateMap<PostMediumRequest, PostMedium>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id during mapping

        // Map from PostMedium to PostMediumRequest
        CreateMap<PostMedium, PostMediumRequest>();

        // Map from PostComment to PostCommentRequest
        CreateMap<PostComment, PostCommentRequest>()
            .ForMember(dest => dest.LikedByUser, opt => opt.Ignore()) // Handle likes logic separately
            .ForMember(dest => dest.TotalLikes, opt => opt.MapFrom(src => src.PostCommentLikes.Count))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.UserProfilePictureUrl, opt => opt.MapFrom(src => src.User.ProfilePictureUrl));


        // Map from PostCommentRequest to PostComment
        CreateMap<PostCommentRequest, PostComment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for auto-generation
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow)); // Set creation time

        ApplyMappingFromAssembly(Assembly.GetExecutingAssembly());
        CreateMap<Place, PlaceVM>()
            .ForMember(dest => dest.PlaceMedia, opt => opt.MapFrom(src => src.PlaceMedia))
            .ForMember(dest => dest.PlaceActivities, opt => opt.MapFrom(src => src.PlaceActivities))
            .ForMember(dest => dest.PlaceTranslation, opt => opt.MapFrom(src => src.PlaceTranslations))
            .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Ward.WardName))
             .ForMember(dest => dest.Distance, opt => opt.Ignore())
             .ForMember(dest => dest.Rating, opt => opt.Ignore())
             .ForMember(dest => dest.TotalPlaceFeedback, opt => opt.Ignore());
        CreateMap<Event, EventRequest>();
        CreateMap<Event, EventViewModel>()
                          .ForMember(dest => dest.PlaceName, opt => opt.MapFrom(src => src.Place.PlaceTranslations.FirstOrDefault().Name));
        CreateMap<PlaceActivity, PlaceActivityRequest>()
        .ForMember(dest => dest.PhotoDisplay, opt => opt.Ignore());
        CreateMap<PlaceMedium, PlaceMediumRequest>();
        CreateMap<Tag, TagViewModel>();
        CreateMap<PlaceFeeedback, PlaceFeedbackRequest>()
                    .ForMember(dest => dest.PlaceFeeedbackMedia, opt => opt.MapFrom(src => src.PlaceFeeedbackMedia))
                    .ForMember(dest => dest.TotalLike, opt => opt.Ignore())
                    .ForMember(dest => dest.isLiked, opt => opt.Ignore());

        // Map for Schedule and ScheduleRequest
        CreateMap<Schedule, ScheduleRequest>().ReverseMap();

        // Map for Destination and DestinationRequest
        CreateMap<Destination, DestinationRequest>()
        .ForMember(dest => dest.PlaceTranslations,
                   opt => opt.MapFrom(src => src.Place.PlaceTranslations));

        // Map for ScheduleLike and ScheduleLikeRequest
        CreateMap<ScheduleLike, ScheduleLikeRequest>().ReverseMap();

        // Map for UserReport and UserReportRequest
        CreateMap<UserReport, UserReportRequest>().ReverseMap();

        CreateMap<PlaceReport, PlaceReportRequest>().ReverseMap();

        // Add mapping for UserPreferenceTagsRequest to UserPreferenceTags
        CreateMap<UserPreferenceTagsRequest, UserPreferenceTags>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // If Id should be auto-generated or managed separately

        CreateMap<ModTagRequest, ModTag>().ReverseMap();

        // Create a mapping between CreateScheduleRequest and Schedule
        CreateMap<CreateScheduleRequest, Schedule>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic));

        CreateMap<CreateDestinationRequest, Destination>()
            .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
            .ForMember(dest => dest.PlaceId, opt => opt.MapFrom(src => src.PlaceId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.IsArrived, opt => opt.MapFrom(src => src.IsArrived))
            .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Detail));

        CreateMap<User, UserFollowVM>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.UserProfileUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl));

        CreateMap<PostRequest, CreatePostRequest>();
    }
    public static IFormFile ConvertToFormFile(string filePath)
    {
        // Kiểm tra nếu tệp tồn tại
        if (File.Exists(filePath))
        {
            var fileBytes = File.ReadAllBytes(filePath);
            var memoryStream = new MemoryStream(fileBytes);
            return new FormFile(memoryStream, 0, memoryStream.Length, "file", Path.GetFileName(filePath));
        }

        return null; // Trả về null nếu không có tệp
    }


    private void ApplyMappingFromAssembly(Assembly assembly)
    {
        var mapFromType = typeof(IMapFrom<>);

        var mappingMethodName = nameof(IMapFrom<object>.Mapping);

        bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

        var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

        var argumentTypes = new Type[] { typeof(Profile) };

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, new object[] { this });
            }
            else
            {
                var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (var @interface in interfaces)
                    {
                        var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(instance, new object[] { this });
                    }
                }
            }
        }
    }
}