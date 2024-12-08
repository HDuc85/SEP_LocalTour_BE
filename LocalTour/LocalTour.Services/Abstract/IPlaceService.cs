﻿using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
        public interface IPlaceService
        {
                Task<PlaceRequest> CreatePlace(PlaceRequest place);
                Task<PaginatedList<PlaceVM>> GetAllPlace(GetPlaceRequest request);
                Task<PlaceDetailModel> GetPlaceById(string languageCode, int placeid);
                Task<PlaceUpdateRequest> UpdatePlace(int placeid, PlaceUpdateRequest request);
                Task<Place> ChangeStatusPlace(int placeid, string status);
                Task<bool> DeletePlace(int placeid);
                Task<List<TagViewModel>> GetTagsByPlaceId(int placeid);
                Task<PaginatedList<PlaceVM>> GetAllPlaceByRole(GetPlaceRequest request);
        }
}
