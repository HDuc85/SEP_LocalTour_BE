using LocalTour.Data;
using LocalTour.Data.Abstract;
using LocalTour.Services.Abstract;
using LocalTour.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LocalTour.WebApi.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IPlaceActivityService, PlaceActivityService>();
        services.AddScoped<IPlaceService, PlaceService>();
        services.AddScoped<IPlaceFeedbackService, PlaceFeedbackService>();
        return services;
    }
}