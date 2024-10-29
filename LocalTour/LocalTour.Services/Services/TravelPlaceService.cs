using LocalTour.Data.Abstract;
using LocalTour.Services.Abstract;

namespace LocalTour.Services.Services;

public class TravelPlaceService : ITravelPlaceService
{
    private readonly IUnitOfWork _unitOfWork;

    public TravelPlaceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    
}