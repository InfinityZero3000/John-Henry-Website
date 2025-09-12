namespace JohnHenryFashionWeb.Services
{
    public interface ISeoService
    {
        SeoData GetDefaultSeoData();
        SeoData GetProductSeoData(string productName, string description, string imageUrl, decimal price);
        SeoData GetCategorySeoData(string categoryName, string description, int productCount);
        SeoData GetPageSeoData(string title, string description, string keywords = "");
        string GenerateStructuredData(object data);
    }

    public class SeoService : ISeoService
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public SeoService(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseUrl = _configuration["SiteSettings:BaseUrl"] ?? "https://johnhenryfashion.com";
        }

        public SeoData GetDefaultSeoData()
        {
            return new SeoData
            {
                Title = "John Henry Fashion - Thời Trang Nam Nữ Cao Cấp",
                Description = "Khám phá bộ sưu tập thời trang nam nữ cao cấp tại John Henry Fashion. Áo sơ mi, vest, đầm, phụ kiện thời trang chính hãng với chất lượng tuyệt vời.",
                Keywords = "thời trang, quần áo, nam, nữ, john henry, fashion, áo sơ mi, vest, đầm, phụ kiện",
                ImageUrl = $"{_baseUrl}/images/logo-og.png",
                Url = _baseUrl,
                Type = "website",
                SiteName = "John Henry Fashion"
            };
        }

        public SeoData GetProductSeoData(string productName, string description, string imageUrl, decimal price)
        {
            return new SeoData
            {
                Title = $"{productName} - John Henry Fashion",
                Description = $"{description?.Substring(0, Math.Min(description.Length, 160))}...",
                Keywords = $"{productName}, thời trang, john henry, quần áo, mua online",
                ImageUrl = !string.IsNullOrEmpty(imageUrl) ? $"{_baseUrl}{imageUrl}" : $"{_baseUrl}/images/logo-og.png",
                Url = _baseUrl,
                Type = "product",
                SiteName = "John Henry Fashion",
                ProductData = new ProductSeoData
                {
                    Name = productName,
                    Description = description,
                    Price = price,
                    Currency = "VND",
                    Availability = "InStock",
                    Condition = "NewCondition"
                }
            };
        }

        public SeoData GetCategorySeoData(string categoryName, string description, int productCount)
        {
            return new SeoData
            {
                Title = $"{categoryName} - John Henry Fashion",
                Description = $"Khám phá {productCount} sản phẩm {categoryName.ToLower()} tại John Henry Fashion. {description}",
                Keywords = $"{categoryName}, thời trang, john henry, quần áo",
                ImageUrl = $"{_baseUrl}/images/logo-og.png",
                Url = _baseUrl,
                Type = "website",
                SiteName = "John Henry Fashion"
            };
        }

        public SeoData GetPageSeoData(string title, string description, string keywords = "")
        {
            return new SeoData
            {
                Title = $"{title} - John Henry Fashion",
                Description = description,
                Keywords = $"{keywords}, john henry, thời trang",
                ImageUrl = $"{_baseUrl}/images/logo-og.png",
                Url = _baseUrl,
                Type = "website",
                SiteName = "John Henry Fashion"
            };
        }

        public string GenerateStructuredData(object data)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });
            }
            catch
            {
                return "{}";
            }
        }
    }

    public class SeoData
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = "website";
        public string SiteName { get; set; } = string.Empty;
        public ProductSeoData? ProductData { get; set; }
    }

    public class ProductSeoData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "VND";
        public string Availability { get; set; } = "InStock";
        public string Condition { get; set; } = "NewCondition";
    }
}
