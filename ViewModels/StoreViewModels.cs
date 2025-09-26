using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    public class StoreLocatorViewModel
    {
        public List<StoreItemViewModel> Stores { get; set; } = new();
        public string? SelectedProvince { get; set; }
        public string? SelectedCity { get; set; }
        public string? SelectedBrand { get; set; }
        public string? SelectedStoreType { get; set; }
        public string? SearchTerm { get; set; }
        
        // Filter options
        public List<string> Provinces { get; set; } = new();
        public List<string> Cities { get; set; } = new();
        public List<string> Brands { get; set; } = new();
        public List<string> StoreTypes { get; set; } = new();
        
        // Map settings
        public double DefaultLatitude { get; set; } = 15.9749327; // Center of Vietnam
        public double DefaultLongitude { get; set; } = 105.8369637;
        public int DefaultZoom { get; set; } = 6;
    }

    public class StoreItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string StoreType { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? WorkingHours { get; set; }
        public string? Email { get; set; }
        public string? GoogleMapIframe { get; set; }
        
        // Display properties
        public string DisplayPhone => !string.IsNullOrEmpty(Phone) ? FormatPhone(Phone) : "Chưa có thông tin";
        public string DisplayWorkingHours => !string.IsNullOrEmpty(WorkingHours) ? WorkingHours : "8:30 - 22:00";
        public string DisplayAddress => $"{Address}, {City}, {Province}";
        public string BrandDisplay => Brand.Replace("John Henry", "").Replace("Freelancer", "").Trim();
        public string StoreTypeDisplay => StoreType.Replace("Cửa hàng nhượng quyền tại", "Cửa hàng").Replace("Đối tác phân phối tại", "Đại lý");
        public bool HasGoogleMap => !string.IsNullOrEmpty(GoogleMapIframe);
        
        private string FormatPhone(string phone)
        {
            // Format phone number: 0123456789 -> 0123.456.789
            if (phone.Length == 10 && phone.StartsWith("0"))
            {
                return $"{phone.Substring(0, 4)}.{phone.Substring(4, 3)}.{phone.Substring(7, 3)}";
            }
            return phone;
        }
    }

    public class StoreDetailsViewModel
    {
        public StoreItemViewModel Store { get; set; } = new();
        public List<StoreItemViewModel> NearbyStores { get; set; } = new();
    }

    public class StoreSearchResultViewModel
    {
        public List<StoreItemViewModel> Results { get; set; } = new();
        public int TotalCount { get; set; }
        public string? SearchTerm { get; set; }
        public bool HasResults => Results.Any();
    }
}
