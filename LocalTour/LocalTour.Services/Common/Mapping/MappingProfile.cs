using System.Reflection;
using AutoMapper;
using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using LocalTour.Services.ViewModel;


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
        CreateMap<Place, PlaceRequest>();
        CreateMap<Event, EventRequest>();
        CreateMap<PlaceActivity, PlaceActivityRequest>();

        // Map for Schedule and ScheduleRequest
        CreateMap<Schedule, ScheduleRequest>().ReverseMap();

        // Map for Destination and DestinationRequest
        CreateMap<Destination, DestinationRequest>().ReverseMap();

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