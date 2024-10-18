using AutoMapper;
using Azure;
using Azure.Core;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PlaceService (IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<PlaceRequest> CreatePlace(PlaceRequest place)
        {
            if (place == null)
            {
                throw new ArgumentNullException(nameof(place));
            }
            var placeEntity = new Place
            {
                TimeOpen = place.TimeOpen,
                TimeClose = place.TimeClose,
                Longitude = place.Longitude,
                Latitude = place.Latitude,
                PhotoDisplay = place.PhotoDisplay,
            };
            await _unitOfWork.RepositoryPlace.Insert(placeEntity);
            await _unitOfWork.CommitAsync();
            foreach (var tags in place.Tags)
            {
                var tagEntity = await _unitOfWork.RepositoryTag.GetById(tags);
                if (tagEntity != null)
                {
                    var placetag = new PlaceTag
                    {
                        PlaceId = placeEntity.Id,
                        TagId = tags,
                    };
                    placeEntity.PlaceTags.Add(placetag);
                };
                
            }
            foreach (var translation in place.PlaceTranslation)
            {
                var translationEntity = new PlaceTranslation
                {
                    LanguageCode = translation.LanguageCode,
                    Name = translation.Name,
                    Description = translation.Description,
                    Address = translation.Address,
                    Contact = translation.Contact
                };
                placeEntity.PlaceTranslations.Add(translationEntity);
            }
            await _unitOfWork.CommitAsync();
            return place;
        }

        public async Task<PaginatedList<PlaceRequest>> GetAllPlace(GetPlaceRequest request)
        {
            var places = _unitOfWork.RepositoryPlace.GetAll().Include(x => x.PlaceTranslations)
           .AsQueryable();

            if (request.SearchTerm is not null)
            {
                places = places.Where(x => x.PlaceTranslations.Any(pt => pt.Name.Contains(request.SearchTerm)) ||
                                           x.PlaceTranslations.Any(pt => pt.Address.Contains(request.SearchTerm)));
            }
            var placesWithDistance = places.Select(place => new
            {
                Place = place,
                Distance = QueryableExtensions.GetDistance(request.CurrentLatitude, request.CurrentLongitude, place.Latitude, place.Longitude) // Đảm bảo Place có Latitude và Longitude
            });

            // Sắp xếp theo khoảng cách
            placesWithDistance = placesWithDistance.OrderBy(x => x.Distance);

            // Chuyển đổi lại về IQueryable<Place>
            var sortedPlaces = placesWithDistance.Select(x => x.Place).AsQueryable();
            return await places
                .ListPaginateWithSortAsync<Place, PlaceRequest>(
                    request.Page,
                    request.Size,
                    request.SortBy,
                    request.SortOrder,
                    _mapper.ConfigurationProvider);
        }

    }
    
}
