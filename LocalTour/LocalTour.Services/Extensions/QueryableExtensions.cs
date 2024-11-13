
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
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

    public static async Task<PaginatedList<TEntityDto>> ListPaginateWithSortPostAsync<TEntity, TEntityDto>(
       this IQueryable<TEntity> items,
       int? page,
       int? size,
       string? sortOrder,
       AutoMapper.IConfigurationProvider mapperConfiguration)
       where TEntityDto : IMapFrom<TEntity>
    {
        sortOrder ??= "asc"; // Default sort order
        var pageNumber = page.GetValueOrDefault(1);
        var sizeNumber = size.GetValueOrDefault(10);

        // Count the total items
        var count = await items.CountAsync();

        // Apply pagination
        var list = await items
            .Paginate(pageNumber, sizeNumber)
            .ToListAsync();

        // Map the items to DTOs
        var mapper = ((AutoMapper.MapperConfiguration)mapperConfiguration).CreateMapper();
        var result = mapper.Map<List<TEntityDto>>(list);

        // Create a paginated list that includes the items and pagination metadata
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
    public static async Task<PaginatedList<TEntityDto>> ListPaginateWithSortPlaceAsync<TEntity, TEntityDto>(
this IQueryable<TEntity> items,
int? page,
int? size,
string? sortBy,
string? sortOrder,
double longitude,
double latitude,
List<int> userTags,
AutoMapper.IConfigurationProvider mapperConfiguration
)
where TEntityDto : IMapFrom<TEntity>
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            // Set a default sorting property if sortBy is null or invalid
            sortBy = typeof(TEntity) == typeof(Place) ? nameof(Place.Id) :
                     typeof(TEntity) == typeof(PlaceTranslation) ? nameof(PlaceTranslation.Id) :
                     typeof(TEntity) == typeof(Event) ? nameof(Event.Id) :
                     typeof(TEntity) == typeof(PlaceActivity) ? nameof(PlaceActivity.Id) :
                     typeof(TEntity) == typeof(PlaceFeeedback) ? nameof(PlaceFeeedback.Id) :
                     throw new ArgumentException("Invalid sortBy property.");
        }
        if(sortOrder == null)
        {
        sortOrder ??= "asc";
        }  
        var pageNumber = page.GetValueOrDefault(1);
        var sizeNumber = size.GetValueOrDefault(10);

        var count = await items.CountAsync();
        if (sortBy.Equals("đề xuất", StringComparison.OrdinalIgnoreCase))
        {
            var lists = items
    .Where(item => (item as Place).PlaceTags.Any(pt => userTags.Contains(pt.TagId)))
    .AsEnumerable()
    .OrderBy(item =>
        CalculateDistance(latitude, longitude, (item as Place).Latitude, (item as Place).Longitude)
    ).Skip((pageNumber - 1) * sizeNumber)
         .Take(sizeNumber)
         .ToList();

            var mappers = mapperConfiguration.CreateMapper();
            var results = mappers.Map<List<TEntityDto>>(lists);
            return new PaginatedList<TEntityDto>(results, count, pageNumber, sizeNumber);
        }
          var places = await items.OfType<Place>().ToListAsync();
        if (sortBy.Equals("khoảng cách", StringComparison.OrdinalIgnoreCase))
        {
            if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                places = places
          .Select(place => new
          {
              Place = place,
              Distance = CalculateDistance(latitude, longitude, place.Latitude, place.Longitude)
          })
          .OrderBy(x => x.Distance)
          .Select(x => x.Place)
          .ToList();
            } else
            {
                places = places
        .Select(place => new
        {
            Place = place,
            Distance = CalculateDistance(latitude, longitude, place.Latitude, place.Longitude)
        })
        .OrderByDescending(x => x.Distance)
        .Select(x => x.Place)
        .ToList();
            }
            var paginatedList = places
         .Skip((pageNumber - 1) * sizeNumber)
         .Take(sizeNumber)
         .ToList();

            var mappers = mapperConfiguration.CreateMapper();
            var results = mappers.Map<List<TEntityDto>>(paginatedList);
            return new PaginatedList<TEntityDto>(results, count, pageNumber, sizeNumber);
        }
        sortOrder ??= "asc";
        var list = await items
            .OrderByCustom(sortBy, sortOrder)
            .Paginate(pageNumber, sizeNumber)
            .ToListAsync();

        var mapper = mapperConfiguration.CreateMapper();
        var result = mapper.Map<List<TEntityDto>>(list);
        return new PaginatedList<TEntityDto>(result, count, pageNumber, sizeNumber);
    }
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of Earth in kilometers
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance in kilometers
    }
}
