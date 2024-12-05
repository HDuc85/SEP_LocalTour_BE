using LocalTour.Domain.Entities;

namespace LocalTour.Services.Abstract;

public interface IAddressService
{
        Task<List<ProvinceNcity>> GetAllProvincesAsync();
        Task<List<DistrictNcity>> GetAllDistrictsAsync(int provinceId);
        Task<List<Ward>> GetAllWardAsync(int cityId);
        Task<DistrictNcity> GetDistrictByWardId(int wardId);
        Task<ProvinceNcity> GetProvinceByDistrictId(int districtId);
}