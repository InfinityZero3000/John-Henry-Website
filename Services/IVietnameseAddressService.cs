using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.Services
{
    public interface IVietnameseAddressService
    {
        Task<List<ProvinceDto>> GetProvincesAsync();
        Task<List<DistrictDto>> GetDistrictsByProvinceAsync(string provinceCode);
        Task<List<WardDto>> GetWardsByDistrictAsync(string districtCode);
    }

    public class ProvinceDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class DistrictDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ProvinceCode { get; set; } = string.Empty;
    }

    public class WardDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DistrictCode { get; set; } = string.Empty;
    }
}
