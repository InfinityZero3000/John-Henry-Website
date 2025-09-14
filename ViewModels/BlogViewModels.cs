using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    public class BlogCategoryViewModel
    {
        public BlogCategory? Category { get; set; }
        public List<BlogPost> Posts { get; set; } = new();
        public List<BlogCategory> AllCategories { get; set; } = new();
        public List<BlogPost> FeaturedPosts { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalPosts { get; set; }
    }
}
