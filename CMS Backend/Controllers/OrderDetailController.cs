using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;

namespace CMS.Backend.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orderDetails = _context.OrderDetails
                                       .Include(od => od.Order)
                                       .Include(od => od.Product)
                                       .ToList();

            return View(orderDetails);
        }
    }
}