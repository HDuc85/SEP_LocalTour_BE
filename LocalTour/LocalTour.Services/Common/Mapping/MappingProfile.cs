using System.Reflection;
using AutoMapper;
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
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => new List<int>())); // If Tags exists in Post

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
            .ForMember(dest => dest.TotalLikes, opt => opt.MapFrom(src => src.PostCommentLikes.Count)); // Assuming you want to count likes

        // Map from PostCommentRequest to PostComment
        CreateMap<PostCommentRequest, PostComment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for auto-generation
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow)); // Set creation time
        ApplyMappingFromAssembly(Assembly.GetExecutingAssembly());
        CreateMap<Place, PlaceVM>()
            .ForMember(dest => dest.PlaceMedia, opt => opt.MapFrom(src => src.PlaceMedia))
            .ForMember(dest => dest.PlaceActivities, opt => opt.MapFrom(src => src.PlaceActivities));
        CreateMap<Event, EventRequest>();
        CreateMap<PlaceActivity, PlaceActivityRequest>()
        .ForMember(dest => dest.PhotoDisplay, opt => opt.Ignore());
        CreateMap<PlaceMedium, PlaceMediumRequest>();

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