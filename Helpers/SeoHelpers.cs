using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using JohnHenryFashionWeb.Services;

namespace JohnHenryFashionWeb.Helpers
{
    public static class SeoHelpers
    {
        public static IHtmlContent GenerateMetaTags(this IHtmlHelper htmlHelper, SeoData seoData)
        {
            var metaTags = new List<string>
            {
                $"<title>{HtmlEncode(seoData.Title)}</title>",
                $"<meta name=\"description\" content=\"{HtmlEncode(seoData.Description)}\" />",
                $"<meta name=\"keywords\" content=\"{HtmlEncode(seoData.Keywords)}\" />",
                
                // Open Graph Meta Tags
                $"<meta property=\"og:title\" content=\"{HtmlEncode(seoData.Title)}\" />",
                $"<meta property=\"og:description\" content=\"{HtmlEncode(seoData.Description)}\" />",
                $"<meta property=\"og:image\" content=\"{HtmlEncode(seoData.ImageUrl)}\" />",
                $"<meta property=\"og:url\" content=\"{HtmlEncode(seoData.Url)}\" />",
                $"<meta property=\"og:type\" content=\"{HtmlEncode(seoData.Type)}\" />",
                $"<meta property=\"og:site_name\" content=\"{HtmlEncode(seoData.SiteName)}\" />",
                
                // Twitter Card Meta Tags
                "<meta name=\"twitter:card\" content=\"summary_large_image\" />",
                $"<meta name=\"twitter:title\" content=\"{HtmlEncode(seoData.Title)}\" />",
                $"<meta name=\"twitter:description\" content=\"{HtmlEncode(seoData.Description)}\" />",
                $"<meta name=\"twitter:image\" content=\"{HtmlEncode(seoData.ImageUrl)}\" />",
                
                // Additional SEO Meta Tags
                "<meta name=\"robots\" content=\"index, follow\" />",
                "<meta name=\"author\" content=\"John Henry Fashion\" />",
                "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />",
                "<meta charset=\"UTF-8\" />",
                "<link rel=\"canonical\" href=\"" + HtmlEncode(seoData.Url) + "\" />"
            };

            return new HtmlString(string.Join("\n", metaTags));
        }

        public static IHtmlContent GenerateStructuredData(this IHtmlHelper htmlHelper, SeoData seoData)
        {
            if (seoData.ProductData != null)
            {
                var structuredData = new
                {
                    context = "https://schema.org/",
                    type = "Product",
                    name = seoData.ProductData.Name,
                    description = seoData.ProductData.Description,
                    image = seoData.ImageUrl,
                    offers = new
                    {
                        type = "Offer",
                        price = seoData.ProductData.Price,
                        priceCurrency = seoData.ProductData.Currency,
                        availability = "https://schema.org/" + seoData.ProductData.Availability,
                        itemCondition = "https://schema.org/" + seoData.ProductData.Condition
                    },
                    brand = new
                    {
                        type = "Brand",
                        name = "John Henry Fashion"
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(structuredData, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });

                return new HtmlString($"<script type=\"application/ld+json\">{json}</script>");
            }

            var organizationData = new
            {
                context = "https://schema.org/",
                type = "Organization",
                name = "John Henry Fashion",
                url = seoData.Url,
                logo = seoData.ImageUrl,
                sameAs = new[]
                {
                    "https://www.facebook.com/JOHNHENRYVN",
                    "https://www.instagram.com/JOHNHENRYVN",
                    "https://www.twitter.com/JOHNHENRYVN"
                }
            };

            var orgJson = System.Text.Json.JsonSerializer.Serialize(organizationData, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            return new HtmlString($"<script type=\"application/ld+json\">{orgJson}</script>");
        }

        public static IHtmlContent GenerateBreadcrumbStructuredData(this IHtmlHelper htmlHelper, List<BreadcrumbItem> breadcrumbs)
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
                }).ToArray()
            };

            var json = System.Text.Json.JsonSerializer.Serialize(breadcrumbList, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            return new HtmlString($"<script type=\"application/ld+json\">{json}</script>");
        }

        public static IHtmlContent GenerateCanonicalUrl(this IHtmlHelper htmlHelper, string url)
        {
            return new HtmlString($"<link rel=\"canonical\" href=\"{HtmlEncode(url)}\" />");
        }

        public static IHtmlContent GenerateHreflangTags(this IHtmlHelper htmlHelper, Dictionary<string, string> alternatives)
        {
            var tags = alternatives.Select(kvp => 
                $"<link rel=\"alternate\" hreflang=\"{HtmlEncode(kvp.Key)}\" href=\"{HtmlEncode(kvp.Value)}\" />");
            
            return new HtmlString(string.Join("\n", tags));
        }

        public static IHtmlContent GenerateRobotsMeta(this IHtmlHelper htmlHelper, bool index = true, bool follow = true, bool archive = true, bool snippet = true)
        {
            var directives = new List<string>();
            
            directives.Add(index ? "index" : "noindex");
            directives.Add(follow ? "follow" : "nofollow");
            
            if (!archive) directives.Add("noarchive");
            if (!snippet) directives.Add("nosnippet");
            
            var content = string.Join(", ", directives);
            return new HtmlString($"<meta name=\"robots\" content=\"{content}\" />");
        }

        public static IHtmlContent GenerateArticleStructuredData(this IHtmlHelper htmlHelper, string title, string content, string author, DateTime publishDate, string? imageUrl = null)
        {
            var article = new
            {
                context = "https://schema.org/",
                type = "Article",
                headline = title,
                articleBody = content,
                author = new
                {
                    type = "Person",
                    name = author
                },
                publisher = new
                {
                    type = "Organization",
                    name = "John Henry Fashion",
                    logo = new
                    {
                        type = "ImageObject",
                        url = imageUrl ?? "/images/logo.png"
                    }
                },
                datePublished = publishDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                image = imageUrl
            };

            var json = System.Text.Json.JsonSerializer.Serialize(article, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            return new HtmlString($"<script type=\"application/ld+json\">{json}</script>");
        }

        public static IHtmlContent GenerateWebsiteStructuredData(this IHtmlHelper htmlHelper, string name, string url, string searchUrl)
        {
            var website = new
            {
                context = "https://schema.org/",
                type = "WebSite",
                name = name,
                url = url,
                potentialAction = new
                {
                    type = "SearchAction",
                    target = new
                    {
                        type = "EntryPoint",
                        urlTemplate = searchUrl
                    },
                    queryInput = "required name=search_term_string"
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(website, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            return new HtmlString($"<script type=\"application/ld+json\">{json}</script>");
        }

        public static IHtmlContent GenerateOrganizationStructuredData(this IHtmlHelper htmlHelper, string name, string url, string logoUrl, string description, object contactInfo)
        {
            var organization = new
            {
                context = "https://schema.org/",
                type = "Organization",
                name = name,
                url = url,
                logo = logoUrl,
                description = description,
                contactPoint = contactInfo,
                sameAs = new[]
                {
                    "https://www.facebook.com/johnhenryfashion",
                    "https://www.instagram.com/johnhenryfashion",
                    "https://twitter.com/johnhenryfashion"
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(organization, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            return new HtmlString($"<script type=\"application/ld+json\">{json}</script>");
        }

        public static string GenerateSlug(this IHtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            // Convert Vietnamese diacritics to ASCII
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

        private static string RemoveVietnameseDiacritics(string text)
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

        private static string HtmlEncode(string input)
        {
            return System.Net.WebUtility.HtmlEncode(input ?? string.Empty);
        }
    }
}
