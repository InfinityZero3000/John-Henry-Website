using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace JohnHenryFashionWeb.Services
{
    public class VietnameseAddressService : IVietnameseAddressService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<VietnameseAddressService> _logger;
        private const string API_BASE_URL = "https://provinces.open-api.vn/api";
        private const int CACHE_HOURS = 24;

        public VietnameseAddressService(
            HttpClient httpClient,
            IMemoryCache cache,
            ILogger<VietnameseAddressService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<ProvinceDto>> GetProvincesAsync()
        {
            const string cacheKey = "provinces_vn";
            
            if (_cache.TryGetValue(cacheKey, out List<ProvinceDto>? cachedProvinces))
            {
                return cachedProvinces ?? new List<ProvinceDto>();
            }

            try
            {
                var response = await _httpClient.GetAsync($"{API_BASE_URL}/p/");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var apiProvinces = JsonSerializer.Deserialize<List<ApiProvince>>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                var provinces = apiProvinces?.Select(p => new ProvinceDto
                {
                    Code = p.Code?.ToString() ?? "",
                    Name = p.Name ?? ""
                }).ToList() ?? new List<ProvinceDto>();

                _cache.Set(cacheKey, provinces, TimeSpan.FromHours(CACHE_HOURS));
                return provinces;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching provinces from API");
                return new List<ProvinceDto>();
            }
        }

        public async Task<List<DistrictDto>> GetDistrictsByProvinceAsync(string provinceCode)
        {
            if (string.IsNullOrEmpty(provinceCode))
                return new List<DistrictDto>();

            var cacheKey = $"districts_{provinceCode}";
            
            if (_cache.TryGetValue(cacheKey, out List<DistrictDto>? cachedDistricts))
            {
                return cachedDistricts ?? new List<DistrictDto>();
            }

            try
            {
                var response = await _httpClient.GetAsync($"{API_BASE_URL}/p/{provinceCode}?depth=2");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var apiProvince = JsonSerializer.Deserialize<ApiProvinceDetail>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                var districts = apiProvince?.Districts?.Select(d => new DistrictDto
                {
                    Code = d.Code?.ToString() ?? "",
                    Name = d.Name ?? "",
                    ProvinceCode = provinceCode
                }).ToList() ?? new List<DistrictDto>();

                _cache.Set(cacheKey, districts, TimeSpan.FromHours(CACHE_HOURS));
                return districts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching districts for province {ProvinceCode}", provinceCode);
                return new List<DistrictDto>();
            }
        }

        public async Task<List<WardDto>> GetWardsByDistrictAsync(string districtCode)
        {
            if (string.IsNullOrEmpty(districtCode))
                return new List<WardDto>();

            var cacheKey = $"wards_{districtCode}";
            
            if (_cache.TryGetValue(cacheKey, out List<WardDto>? cachedWards))
            {
                return cachedWards ?? new List<WardDto>();
            }

            try
            {
                var response = await _httpClient.GetAsync($"{API_BASE_URL}/d/{districtCode}?depth=2");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var apiDistrict = JsonSerializer.Deserialize<ApiDistrictDetail>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                var wards = apiDistrict?.Wards?.Select(w => new WardDto
                {
                    Code = w.Code?.ToString() ?? "",
                    Name = w.Name ?? "",
                    DistrictCode = districtCode
                }).ToList() ?? new List<WardDto>();

                _cache.Set(cacheKey, wards, TimeSpan.FromHours(CACHE_HOURS));
                return wards;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching wards for district {DistrictCode}", districtCode);
                return new List<WardDto>();
            }
        }

        // Internal API response models
        private class ApiProvince
        {
            public int? Code { get; set; }
            public string? Name { get; set; }
        }

        private class ApiProvinceDetail
        {
            public int? Code { get; set; }
            public string? Name { get; set; }
            public List<ApiDistrict>? Districts { get; set; }
        }

        private class ApiDistrict
        {
            public int? Code { get; set; }
            public string? Name { get; set; }
        }

        private class ApiDistrictDetail
        {
            public int? Code { get; set; }
            public string? Name { get; set; }
            public List<ApiWard>? Wards { get; set; }
        }

        private class ApiWard
        {
            public int? Code { get; set; }
            public string? Name { get; set; }
        }
    }
}
