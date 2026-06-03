using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System.Linq;

namespace CMS.Backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // LINQ: Lấy 3 bài viết mới nhất
            var latestPosts = _context.Posts
                              .Include(p => p.Category) // Lấy kèm tên danh mục
                              .OrderByDescending(p => p.CreatedDate) // Mới nhất lên đầu
                              .Take(3) // Lấy 3 bài viết
                              .ToList();

            return View(latestPosts);
        }
    }
}