using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.payOS.Types;

namespace LocalTour.Services.Abstract
{
        public interface IPlaceService
        {
                Task<PlaceRequest> CreatePlace(PlaceRequest place);
                Task<PaginatedList<PlaceVM>> GetAllPlace(GetPlaceRequest request);
                Task<PlaceDetailModel> GetPlaceById(string languageCode, int placeid);
                Task<PlaceUpdateRequest> UpdatePlace(int placeid, PlaceUpdateRequest request);
                Task<Place> ChangeStatusPlace(int placeid, string status);
                Task<Place> TransferAuthor(int placeId, Guid userIdTransfer);
                Task<bool> DeletePlace(int placeid);
                Task<List<TagViewModel>> GetTagsByPlaceId(int placeid);
                Task<PaginatedList<PlaceVM>> GetAllPlaceByRole(GetPlaceRequest request);
                Task<CreatePaymentResult> CreatePaymentPlaceRegister(int placeId, string userId);
                Task<string> PlaceSuccessPayment(long orderCode, string status);
                Task<string> PlaceCancelPayment(long orderCode, string status);
                Task<bool> sendMail(SendMailRequest request);
                Task<PaginatedList<PlaceVM>> GetAllPlaceAuthentic(GetPlaceRequest request);
                Task<Place> ChangeStatusApproved(int placeid, string status);
                Task<Place> ChangeStatusAuthentic(int placeid, string status);
                Task<PaginatedList<PlaceVM>> GetAllPlaceByAuthentic(GetPlaceRequest request);
        }
}
