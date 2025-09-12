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
                    "https://www.facebook.com/johnhenryfashion",
                    "https://www.instagram.com/johnhenryfashion",
                    "https://www.twitter.com/johnhenryfashion"
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

        private static string HtmlEncode(string input)
        {
            return System.Net.WebUtility.HtmlEncode(input ?? string.Empty);
        }
    }

    public class BreadcrumbItem
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
