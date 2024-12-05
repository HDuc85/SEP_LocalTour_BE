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

    public async Task<DistrictNcity> GetDistrictByWardId(int wardId)
    {
        var ward = await _unitOfWork.RepositoryWard.GetById(wardId);
        if (ward == null)
        {
            throw new Exception($"Ward with ID {wardId} not found.");
        }
        var district = await _unitOfWork.RepositoryDistrictNcity.GetById(ward.DistrictNcityId);
        if (district == null)
        {
            throw new Exception($"District with ID {ward.DistrictNcityId} not found.");
        }
        return district;
    }

    public async Task<ProvinceNcity> GetProvinceByDistrictId(int districtId)
    {
        var district = await _unitOfWork.RepositoryDistrictNcity.GetById(districtId);
        if (district == null)
        {
            throw new Exception($"District with ID {districtId} not found.");
        }
        var province = await _unitOfWork.RepositoryProvinceNcity.GetById(district.ProvinceNcityId);
        if (province == null)
        {
            throw new Exception($"Province with ID {district.ProvinceNcityId} not found.");
        }

        return province;
    }
}