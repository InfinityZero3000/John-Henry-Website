using JohnHenryFashionWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace JohnHenryFashionWeb.ViewModels
{
    public class ProductSearchFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Material { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? InStock { get; set; } = true; // Default to show only in-stock items
        public string SortBy { get; set; } = "name"; // name, price_asc, price_desc, newest, popularity, rating
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // For the view
        public List<Category> Categories { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
        public List<string> AvailableColors { get; set; } = new();
        public List<string> AvailableMaterials { get; set; } = new();
        public decimal MinPriceRange { get; set; }
        public decimal MaxPriceRange { get; set; }

        // Results
        public PagedProductListViewModel Results { get; set; } = new();
    }

    public class PagedProductListViewModel
    {
        public List<ProductSearchItemViewModel> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int StartIndex => (PageNumber - 1) * PageSize + 1;
        public int EndIndex => Math.Min(PageNumber * PageSize, TotalCount);
    }

    public class ProductSearchItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? BrandName { get; set; }
        public bool IsFeatured { get; set; }
        public bool InStock { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Material { get; set; }

        // Calculated properties
        public decimal DisplayPrice => SalePrice ?? Price;
        public bool HasDiscount => SalePrice.HasValue && SalePrice < Price;
        public decimal DiscountPercentage => HasDiscount ? Math.Round(((Price - SalePrice!.Value) / Price) * 100) : 0;
        public string PriceFormatted => DisplayPrice.ToString("N0") + " VNĐ";
        public string OriginalPriceFormatted => Price.ToString("N0") + " VNĐ";
    }

    public class ProductQuickSearchViewModel
    {
        public string? Query { get; set; }
        public int Limit { get; set; } = 5;
        public List<ProductQuickSearchResultViewModel> Results { get; set; } = new();
    }

    public class ProductQuickSearchResultViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string PriceFormatted => (SalePrice ?? Price).ToString("N0") + " VNĐ";
    }

    public class ProductComparisonViewModel
    {
        public List<ProductSearchItemViewModel> Products { get; set; } = new();
        public List<string> ComparisonFeatures { get; set; } = new();
    }

    public class ProductFilterSummaryViewModel
    {
        public int TotalProducts { get; set; }
        public string? AppliedFilters { get; set; }
        public List<FilterSummaryItem> FilterItems { get; set; } = new();
    }

    public class FilterSummaryItem
    {
        public string Type { get; set; } = string.Empty; // category, brand, price, etc.
        public string Value { get; set; } = string.Empty;
        public string DisplayValue { get; set; } = string.Empty;
        public string RemoveUrl { get; set; } = string.Empty;
    }
}
