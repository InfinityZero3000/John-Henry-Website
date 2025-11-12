using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;

namespace JohnHenryFashionWeb.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AddressApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AddressApi/provinces
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _context.Provinces
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    id = p.Id,
                    code = p.Code,
                    name = p.Name,
                    fullName = p.FullName
                })
                .ToListAsync();

            return Ok(provinces);
        }

        // GET: api/AddressApi/districts/{provinceId}
        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            var districts = await _context.Districts
                .Where(d => d.ProvinceId == provinceId)
                .OrderBy(d => d.Name)
                .Select(d => new
                {
                    id = d.Id,
                    code = d.Code,
                    name = d.Name,
                    fullName = d.FullName
                })
                .ToListAsync();

            return Ok(districts);
        }

        // GET: api/AddressApi/wards/{districtId}
        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWards(int districtId)
        {
            var wards = await _context.Wards
                .Where(w => w.DistrictId == districtId)
                .OrderBy(w => w.Name)
                .Select(w => new
                {
                    id = w.Id,
                    code = w.Code,
                    name = w.Name,
                    fullName = w.FullName
                })
                .ToListAsync();

            return Ok(wards);
        }
    }
}
