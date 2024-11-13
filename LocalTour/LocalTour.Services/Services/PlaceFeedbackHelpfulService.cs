using Azure.Core;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LocalTour.Services.Services
{
    public class PlaceFeedbackHelpfulService : IPlaceFeedbackHelpfulService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PlaceFeedbackHelpfulService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) 
        { 
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PlaceFeeedbackHelpful> CreateorDeleteHelpful(int placeid, int placefeedbackid)
        {
            var places = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (places == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            if (placefeedbackid == null)
            {
                throw new ArgumentNullException(nameof(placefeedbackid));
            }
            
            var user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(user) || !Guid.TryParse(user, out var userId))
            {
                throw new UnauthorizedAccessException("User not found or invalid User ID.");
            }
            var reaction = await _unitOfWork.RepositoryPlaceFeeedbackHelpful
                                                .GetAll()
                                                .FirstOrDefaultAsync(x => x.UserId == userId && x.PlaceFeedBackId == placefeedbackid);
            if (reaction == null)
            {
                  var helpful = new PlaceFeeedbackHelpful
                            {
                                UserId = userId,
                                PlaceFeedBackId = placefeedbackid,
                                CreatedDate = DateTime.UtcNow,
                            }; 
                await _unitOfWork.RepositoryPlaceFeeedbackHelpful.Insert(helpful);
            }
            else
            {
                _unitOfWork.RepositoryPlaceFeeedbackHelpful.Delete(reaction);

            }
            await _unitOfWork.CommitAsync();
            return reaction;
        }
    }
}
