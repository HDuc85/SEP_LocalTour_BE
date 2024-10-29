using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using LocalTour.Services.ViewModel;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace LocalTour.Services.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> items, int page, int size)
    {
        return items.Skip((page - 1) * size).Take(size);
    }

    public static async Task<PaginatedList<TEntityDto>> ListPaginateWithSortAsync<TEntity, TEntityDto>(
    this IQueryable<TEntity> items,
    int? page,
    int? size,
    string? sortBy,
    string? sortOrder,
    AutoMapper.IConfigurationProvider mapperConfiguration
   )
    where TEntityDto : IMapFrom<TEntity>
    {
        if (string.IsNullOrEmpty(sortBy) || !IsValidProperty<TEntityDto>(sortBy))
        {
            // Set a default sorting property if sortBy is null or invalid
            sortBy = typeof(TEntity) == typeof(Place) ? nameof(Place.Id) :
                     typeof(TEntity) == typeof(PlaceTranslation) ? nameof(PlaceTranslation.Id) :
                     typeof(TEntity) == typeof(Event) ? nameof(Event.Id) :
                     typeof(TEntity) == typeof(PlaceActivity) ? nameof(PlaceActivity.Id) :
                     typeof(TEntity) == typeof(PlaceFeeedback) ? nameof(PlaceFeeedback.Id) :
                     throw new ArgumentException("Invalid sortBy property.");
        }

        sortOrder ??= "asc";
        var pageNumber = page.GetValueOrDefault(1);
        var sizeNumber = size.GetValueOrDefault(10);

        var count = await items.CountAsync();
        var list = await items
            .OrderByCustom(sortBy, sortOrder)
            .Paginate(pageNumber, sizeNumber)
            .ToListAsync();

        var mapper = mapperConfiguration.CreateMapper();
        var result = mapper.Map<List<TEntityDto>>(list);
        return new PaginatedList<TEntityDto>(result, count, pageNumber, sizeNumber);
    }

    private static bool IsValidProperty<TEntityDto>(string propertyName)
    {
        var propertyInfo = typeof(TEntityDto).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        return propertyInfo != null;
    }

    public static IQueryable<TEntity> OrderByCustom<TEntity>(this IQueryable<TEntity> items, string sortBy, string sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            throw new ArgumentNullException(nameof(sortBy), "Sort by parameter cannot be null or empty.");

        var type = typeof(TEntity);
        var parameter = Expression.Parameter(type, "t");
        var property = type.GetProperty(sortBy);

        if (property == null)
            throw new ArgumentException($"Property '{sortBy}' does not exist on type '{type.Name}'.");

        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);
        var methodName = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

        var result = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { type, property.PropertyType },
            items.Expression,
            Expression.Quote(orderByExpression)
        );

        return items.Provider.CreateQuery<TEntity>(result);
    }
    public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Radius of the Earth in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance in km
    }

    private static double ToRadians(double angle)
    {
        return angle * Math.PI / 180;
    }

}