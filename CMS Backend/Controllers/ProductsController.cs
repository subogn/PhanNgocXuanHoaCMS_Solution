using CMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL + SEARCH + PAGING
        // =========================
        [HttpGet]
        public IActionResult GetAll(
            int page = 1,
            int pageSize = 8,
            string? keyword = null)
        {
            var query = _context.Products.AsQueryable();

            // 🔍 SEARCH
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }

            var totalItems = query.Count();

            var products = query
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    p.ImageUrl,
                    p.IsFeatured,
                    p.IsNew,
                    p.IsBestSeller,
                    p.DiscountPercent,
                    p.CategoryProductId
                })
                .ToList();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                products
            });
        }

        // =========================
        // GET BY CATEGORY
        // =========================
        [HttpGet("categoriesproduct/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var pro = _context.Products
                .Where(p => p.CategoryProductId == categoryId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    p.ImageUrl,
                    p.IsFeatured,
                    p.IsNew,
                    p.IsBestSeller,
                    p.DiscountPercent,
                })
                .ToList();

            return Ok(pro);
        }

        // =========================
        // GET DETAIL
        // =========================
        [HttpGet("{id:int}")]
        public IActionResult GetDetail(int id)
        {
            var pro = _context.Products
                .FirstOrDefault(p => p.Id == id);

            if (pro == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sản phẩm này trong hệ thống"
                });
            }

            return Ok(pro);
        }

        // =========================
        // FEATURED
        // =========================
        [HttpGet("featured")]
        public IActionResult Featured()
        {
            var data = _context.Products
                .Where(x => x.IsFeatured)
                .Take(4)
                .ToList();

            return Ok(data);
        }

        // =========================
        // NEW PRODUCTS
        // =========================
        [HttpGet("new")]
        public IActionResult NewProducts()
        {
            var data = _context.Products
                .Where(x => x.IsNew)
                .Take(4)
                .ToList();

            return Ok(data);
        }

        // =========================
        // BEST SELLER
        // =========================
        [HttpGet("bestseller")]
        public IActionResult BestSeller()
        {
            var data = _context.Products
                .Where(x => x.IsBestSeller)
                .Take(4)
                .ToList();

            return Ok(data);
        }

        // =========================
        // DISCOUNT
        // =========================
        [HttpGet("discount")]
        public IActionResult Discount()
        {
            var data = _context.Products
                .Where(x => x.DiscountPercent > 0)
                .Take(4)
                .ToList();

            return Ok(data);
        }
    }
}