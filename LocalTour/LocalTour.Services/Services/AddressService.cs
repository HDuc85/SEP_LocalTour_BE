using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;

namespace LocalTour.Services.Services;

public class AddressService : IAddressService
{
    readonly private IUnitOfWork _unitOfWork;

    public AddressService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProvinceNcity>> GetAllProvincesAsync()
    {
        return  _unitOfWork.RepositoryProvinceNcity.GetAll().ToList();
    }

    public async Task<List<DistrictNcity>> GetAllDistrictsAsync(int provinceId)
    {
        return _unitOfWork.RepositoryDistrictNcity.GetDataQueryable(x => x.ProvinceNcityId == provinceId).ToList();
    }
    public async Task<List<Ward>> GetAllWardAsync(int cityId)
    {
        return _unitOfWork.RepositoryWard.GetDataQueryable(x => x.DistrictNcityId == cityId).ToList();
    }
}