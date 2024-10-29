
using Microsoft.Extensions.DependencyInjection;

namespace LocalTour.WebApi.Configuration;

public static class ConfigureService
{
    
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddHttpContextAccessor();
        return services;
    }
}