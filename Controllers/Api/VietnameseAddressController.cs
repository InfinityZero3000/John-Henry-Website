using Microsoft.AspNetCore.Mvc;
using JohnHenryFashionWeb.Services;

namespace JohnHenryFashionWeb.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class VietnameseAddressController : ControllerBase
    {
        private readonly IVietnameseAddressService _addressService;
        private readonly ILogger<VietnameseAddressController> _logger;

        public VietnameseAddressController(
            IVietnameseAddressService addressService,
            ILogger<VietnameseAddressController> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        /// <summary>
        /// Get all provinces in Vietnam
        /// </summary>
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                var provinces = await _addressService.GetProvincesAsync();
                return Ok(provinces);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provinces");
                return StatusCode(500, new { message = "Error retrieving provinces" });
            }
        }

        /// <summary>
        /// Get districts by province code
        /// </summary>
        [HttpGet("districts/{provinceCode}")]
        public async Task<IActionResult> GetDistricts(string provinceCode)
        {
            try
            {
                var districts = await _addressService.GetDistrictsByProvinceAsync(provinceCode);
                return Ok(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting districts for province {ProvinceCode}", provinceCode);
                return StatusCode(500, new { message = "Error retrieving districts" });
            }
        }

        /// <summary>
        /// Get wards by district code
        /// </summary>
        [HttpGet("wards/{districtCode}")]
        public async Task<IActionResult> GetWards(string districtCode)
        {
            try
            {
                var wards = await _addressService.GetWardsByDistrictAsync(districtCode);
                return Ok(wards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wards for district {DistrictCode}", districtCode);
                return StatusCode(500, new { message = "Error retrieving wards" });
            }
        }
    }
}
