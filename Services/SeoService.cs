namespace JohnHenryFashionWeb.Services
{
    public interface ISeoService
    {
        SeoData GetDefaultSeoData();
        SeoData GetProductSeoData(string productName, string description, string imageUrl, decimal price, string? category = null, string? brand = null);
        SeoData GetCategorySeoData(string categoryName, string description, int productCount);
        SeoData GetPageSeoData(string title, string description, string keywords = "", string? currentUrl = null);
        SeoData GetBlogSeoData(string title, string content, string? imageUrl = null, DateTime? publishDate = null);
        string GenerateStructuredData(object data);
        string GenerateProductJsonLd(ProductSeoData product, string productUrl);
        string GenerateBreadcrumbJsonLd(List<BreadcrumbItem> breadcrumbs);
        string GenerateOrganizationJsonLd();
        string GenerateWebsiteJsonLd();
        string GenerateSiteLinksSearchBoxJsonLd();
        List<string> ExtractKeywordsFromContent(string content, int maxKeywords = 10);
        string OptimizeMetaDescription(string description, int maxLength = 160);
        string GenerateSlug(string text);
        SeoAnalysisResult AnalyzePage(string title, string description, string content, string? keywords = null);
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

        public SeoData GetProductSeoData(string productName, string description, string imageUrl, decimal price, string? category = null, string? brand = null)
        {
            var optimizedDescription = OptimizeMetaDescription(description ?? "", 160);
            var keywords = ExtractKeywordsFromContent($"{productName} {description} {category} {brand}", 8);
            
            return new SeoData
            {
                Title = $"{productName} - John Henry Fashion",
                Description = optimizedDescription,
                Keywords = string.Join(", ", keywords),
                ImageUrl = !string.IsNullOrEmpty(imageUrl) ? $"{_baseUrl}{imageUrl}" : $"{_baseUrl}/images/logo-og.png",
                Url = _baseUrl,
                Type = "product",
                SiteName = "John Henry Fashion",
                ProductData = new ProductSeoData
                {
                    Name = productName,
                    Description = description ?? "",
                    Price = price,
                    Currency = "VND",
                    Availability = "InStock",
                    Condition = "NewCondition",
                    Category = category,
                    Brand = brand
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

        public SeoData GetPageSeoData(string title, string description, string keywords = "", string? currentUrl = null)
        {
            return new SeoData
            {
                Title = $"{title} - John Henry Fashion",
                Description = OptimizeMetaDescription(description, 160),
                Keywords = $"{keywords}, john henry, thời trang",
                ImageUrl = $"{_baseUrl}/images/logo-og.png",
                Url = currentUrl ?? _baseUrl,
                Type = "website",
                SiteName = "John Henry Fashion"
            };
        }

        public SeoData GetBlogSeoData(string title, string content, string? imageUrl = null, DateTime? publishDate = null)
        {
            var description = OptimizeMetaDescription(content, 160);
            var keywords = ExtractKeywordsFromContent(content, 10);
            
            return new SeoData
            {
                Title = $"{title} - Blog John Henry Fashion",
                Description = description,
                Keywords = string.Join(", ", keywords),
                ImageUrl = imageUrl ?? $"{_baseUrl}/images/logo-og.png",
                Url = _baseUrl,
                Type = "article",
                SiteName = "John Henry Fashion",
                ArticleData = new ArticleSeoData
                {
                    Title = title,
                    Content = content,
                    PublishedDate = publishDate ?? DateTime.Now,
                    Author = "John Henry Fashion"
                }
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

        public string GenerateProductJsonLd(ProductSeoData product, string productUrl)
        {
            var productJsonLd = new
            {
                context = "https://schema.org/",
                type = "Product",
                name = product.Name,
                description = product.Description,
                price = product.Price,
                priceCurrency = product.Currency,
                availability = $"https://schema.org/{product.Availability}",
                condition = $"https://schema.org/{product.Condition}",
                url = productUrl,
                category = product.Category,
                brand = new
                {
                    type = "Brand",
                    name = product.Brand ?? "John Henry Fashion"
                },
                offers = new
                {
                    type = "Offer",
                    price = product.Price,
                    priceCurrency = product.Currency,
                    availability = $"https://schema.org/{product.Availability}",
                    url = productUrl
                }
            };

            return GenerateStructuredData(productJsonLd);
        }

        public string GenerateBreadcrumbJsonLd(List<BreadcrumbItem> breadcrumbs)
        {
            var breadcrumbList = new
            {
                context = "https://schema.org/",
                type = "BreadcrumbList",
                itemListElement = breadcrumbs.Select((item, index) => new
                {
                    type = "ListItem",
                    position = index + 1,
                    name = item.Name,
                    item = item.Url
                })
            };

            return GenerateStructuredData(breadcrumbList);
        }

        public string GenerateOrganizationJsonLd()
        {
            var organization = new
            {
                context = "https://schema.org/",
                type = "Organization",
                name = "John Henry Fashion",
                url = _baseUrl,
                logo = $"{_baseUrl}/images/logo.png",
                description = "Thương hiệu thời trang nam nữ cao cấp tại Việt Nam",
                address = new
                {
                    type = "PostalAddress",
                    streetAddress = "123 Nguyễn Huệ",
                    addressLocality = "Quận 1",
                    addressRegion = "TP.HCM",
                    postalCode = "70000",
                    addressCountry = "VN"
                },
                contactPoint = new
                {
                    type = "ContactPoint",
                    telephone = "+84-28-1234-5678",
                    contactType = "customer service"
                },
                sameAs = new[]
                {
                    "https://www.facebook.com/johnhenryfashion",
                    "https://www.instagram.com/johnhenryfashion",
                    "https://twitter.com/johnhenryfashion"
                }
            };

            return GenerateStructuredData(organization);
        }

        public string GenerateWebsiteJsonLd()
        {
            var website = new
            {
                context = "https://schema.org/",
                type = "WebSite",
                name = "John Henry Fashion",
                url = _baseUrl,
                description = "Thời trang nam nữ cao cấp",
                potentialAction = new
                {
                    type = "SearchAction",
                    target = new
                    {
                        type = "EntryPoint",
                        urlTemplate = $"{_baseUrl}/products?searchTerm={{search_term_string}}"
                    },
                    queryInput = "required name=search_term_string"
                }
            };

            return GenerateStructuredData(website);
        }

        public string GenerateSiteLinksSearchBoxJsonLd()
        {
            var searchBox = new
            {
                context = "https://schema.org/",
                type = "WebSite",
                url = _baseUrl,
                potentialAction = new
                {
                    type = "SearchAction",
                    target = $"{_baseUrl}/products?searchTerm={{search_term_string}}",
                    queryInput = "required name=search_term_string"
                }
            };

            return GenerateStructuredData(searchBox);
        }

        public List<string> ExtractKeywordsFromContent(string content, int maxKeywords = 10)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new List<string>();

            // Common stop words in Vietnamese
            var stopWords = new HashSet<string>
            {
                "và", "của", "trong", "với", "để", "từ", "có", "được", "là", "một", "các", "cho", "về", "này", "đó",
                "the", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by", "from", "is", "are", "was", "were"
            };

            // Extract words, remove stop words, and get most frequent
            var words = content.ToLower()
                .Split(new char[] { ' ', '.', ',', ';', ':', '!', '?', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2 && !stopWords.Contains(w))
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(maxKeywords)
                .Select(g => g.Key)
                .ToList();

            return words;
        }

        public string OptimizeMetaDescription(string description, int maxLength = 160)
        {
            if (string.IsNullOrWhiteSpace(description))
                return "";

            description = description.Trim();

            if (description.Length <= maxLength)
                return description;

            // Cut at word boundary
            var truncated = description.Substring(0, maxLength);
            var lastSpace = truncated.LastIndexOf(' ');

            if (lastSpace > maxLength - 20) // Only cut at word boundary if it's close to the limit
                truncated = truncated.Substring(0, lastSpace);

            return truncated + "...";
        }

        public string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            // Remove Vietnamese diacritics
            text = RemoveVietnameseDiacritics(text);

            // Convert to lowercase and replace spaces/special chars with hyphens
            text = text.ToLower()
                .Replace(" ", "-")
                .Replace("_", "-");

            // Remove non-alphanumeric characters except hyphens
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\-]", "");

            // Remove multiple consecutive hyphens
            text = System.Text.RegularExpressions.Regex.Replace(text, @"-+", "-");

            // Remove leading/trailing hyphens
            text = text.Trim('-');

            return text;
        }

        public SeoAnalysisResult AnalyzePage(string title, string description, string content, string? keywords = null)
        {
            var result = new SeoAnalysisResult();

            // Analyze title
            result.TitleLength = title.Length;
            result.TitleOptimal = title.Length >= 30 && title.Length <= 60;
            result.TitleIssues = new List<string>();

            if (title.Length < 30)
                result.TitleIssues.Add("Tiêu đề quá ngắn (nên từ 30-60 ký tự)");
            if (title.Length > 60)
                result.TitleIssues.Add("Tiêu đề quá dài (nên từ 30-60 ký tự)");

            // Analyze description
            result.DescriptionLength = description.Length;
            result.DescriptionOptimal = description.Length >= 120 && description.Length <= 160;
            result.DescriptionIssues = new List<string>();

            if (description.Length < 120)
                result.DescriptionIssues.Add("Mô tả quá ngắn (nên từ 120-160 ký tự)");
            if (description.Length > 160)
                result.DescriptionIssues.Add("Mô tả quá dài (nên từ 120-160 ký tự)");

            // Analyze content
            result.ContentLength = content.Length;
            result.ContentOptimal = content.Length >= 300;

            if (content.Length < 300)
                result.ContentIssues.Add("Nội dung quá ngắn (nên trên 300 ký tự)");

            // Keyword analysis
            if (!string.IsNullOrEmpty(keywords))
            {
                var keywordList = keywords.Split(',').Select(k => k.Trim().ToLower()).ToList();
                var contentLower = content.ToLower();
                var titleLower = title.ToLower();

                result.KeywordInTitle = keywordList.Any(k => titleLower.Contains(k));
                result.KeywordInDescription = keywordList.Any(k => description.ToLower().Contains(k));
                result.KeywordDensity = CalculateKeywordDensity(content, keywordList);
            }

            // Calculate overall score
            var score = 0;
            if (result.TitleOptimal) score += 20;
            if (result.DescriptionOptimal) score += 20;
            if (result.ContentOptimal) score += 20;
            if (result.KeywordInTitle) score += 15;
            if (result.KeywordInDescription) score += 15;
            if (result.KeywordDensity >= 1 && result.KeywordDensity <= 3) score += 10;

            result.OverallScore = score;

            return result;
        }

        private string RemoveVietnameseDiacritics(string text)
        {
            var diacritics = new Dictionary<char, char>
            {
                {'à', 'a'}, {'á', 'a'}, {'ạ', 'a'}, {'ả', 'a'}, {'ã', 'a'}, {'â', 'a'}, {'ầ', 'a'}, {'ấ', 'a'}, {'ậ', 'a'}, {'ẩ', 'a'}, {'ẫ', 'a'},
                {'ă', 'a'}, {'ằ', 'a'}, {'ắ', 'a'}, {'ặ', 'a'}, {'ẳ', 'a'}, {'ẵ', 'a'},
                {'è', 'e'}, {'é', 'e'}, {'ẹ', 'e'}, {'ẻ', 'e'}, {'ẽ', 'e'}, {'ê', 'e'}, {'ề', 'e'}, {'ế', 'e'}, {'ệ', 'e'}, {'ể', 'e'}, {'ễ', 'e'},
                {'ì', 'i'}, {'í', 'i'}, {'ị', 'i'}, {'ỉ', 'i'}, {'ĩ', 'i'},
                {'ò', 'o'}, {'ó', 'o'}, {'ọ', 'o'}, {'ỏ', 'o'}, {'õ', 'o'}, {'ô', 'o'}, {'ồ', 'o'}, {'ố', 'o'}, {'ộ', 'o'}, {'ổ', 'o'}, {'ỗ', 'o'},
                {'ơ', 'o'}, {'ờ', 'o'}, {'ớ', 'o'}, {'ợ', 'o'}, {'ở', 'o'}, {'ỡ', 'o'},
                {'ù', 'u'}, {'ú', 'u'}, {'ụ', 'u'}, {'ủ', 'u'}, {'ũ', 'u'}, {'ư', 'u'}, {'ừ', 'u'}, {'ứ', 'u'}, {'ự', 'u'}, {'ử', 'u'}, {'ữ', 'u'},
                {'ỳ', 'y'}, {'ý', 'y'}, {'ỵ', 'y'}, {'ỷ', 'y'}, {'ỹ', 'y'},
                {'đ', 'd'},
                {'À', 'A'}, {'Á', 'A'}, {'Ạ', 'A'}, {'Ả', 'A'}, {'Ã', 'A'}, {'Â', 'A'}, {'Ầ', 'A'}, {'Ấ', 'A'}, {'Ậ', 'A'}, {'Ẩ', 'A'}, {'Ẫ', 'A'},
                {'Ă', 'A'}, {'Ằ', 'A'}, {'Ắ', 'A'}, {'Ặ', 'A'}, {'Ẳ', 'A'}, {'Ẵ', 'A'},
                {'È', 'E'}, {'É', 'E'}, {'Ẹ', 'E'}, {'Ẻ', 'E'}, {'Ẽ', 'E'}, {'Ê', 'E'}, {'Ề', 'E'}, {'Ế', 'E'}, {'Ệ', 'E'}, {'Ể', 'E'}, {'Ễ', 'E'},
                {'Ì', 'I'}, {'Í', 'I'}, {'Ị', 'I'}, {'Ỉ', 'I'}, {'Ĩ', 'I'},
                {'Ò', 'O'}, {'Ó', 'O'}, {'Ọ', 'O'}, {'Ỏ', 'O'}, {'Õ', 'O'}, {'Ô', 'O'}, {'Ồ', 'O'}, {'Ố', 'O'}, {'Ộ', 'O'}, {'Ổ', 'O'}, {'Ỗ', 'O'},
                {'Ơ', 'O'}, {'Ờ', 'O'}, {'Ớ', 'O'}, {'Ợ', 'O'}, {'Ở', 'O'}, {'Ỡ', 'O'},
                {'Ù', 'U'}, {'Ú', 'U'}, {'Ụ', 'U'}, {'Ủ', 'U'}, {'Ũ', 'U'}, {'Ư', 'U'}, {'Ừ', 'U'}, {'Ứ', 'U'}, {'Ự', 'U'}, {'Ử', 'U'}, {'Ữ', 'U'},
                {'Ỳ', 'Y'}, {'Ý', 'Y'}, {'Ỵ', 'Y'}, {'Ỷ', 'Y'}, {'Ỹ', 'Y'},
                {'Đ', 'D'}
            };

            var result = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (diacritics.TryGetValue(c, out char replacement))
                    result.Append(replacement);
                else
                    result.Append(c);
            }

            return result.ToString();
        }

        private double CalculateKeywordDensity(string content, List<string> keywords)
        {
            if (string.IsNullOrEmpty(content) || !keywords.Any())
                return 0;

            var words = content.Split(new char[] { ' ', '.', ',', ';', ':', '!', '?', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var totalWords = words.Length;
            var keywordCount = 0;

            foreach (var word in words)
            {
                if (keywords.Any(k => word.ToLower().Contains(k)))
                    keywordCount++;
            }

            return totalWords > 0 ? (double)keywordCount / totalWords * 100 : 0;
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
        public ArticleSeoData? ArticleData { get; set; }
    }

    public class ProductSeoData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "VND";
        public string Availability { get; set; } = "InStock";
        public string Condition { get; set; } = "NewCondition";
        public string? Category { get; set; }
        public string? Brand { get; set; }
    }

    public class ArticleSeoData
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; } = string.Empty;
    }

    public class BreadcrumbItem
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class SeoAnalysisResult
    {
        public int TitleLength { get; set; }
        public bool TitleOptimal { get; set; }
        public List<string> TitleIssues { get; set; } = new();

        public int DescriptionLength { get; set; }
        public bool DescriptionOptimal { get; set; }
        public List<string> DescriptionIssues { get; set; } = new();

        public int ContentLength { get; set; }
        public bool ContentOptimal { get; set; }
        public List<string> ContentIssues { get; set; } = new();

        public bool KeywordInTitle { get; set; }
        public bool KeywordInDescription { get; set; }
        public double KeywordDensity { get; set; }

        public int OverallScore { get; set; }
    }
}
