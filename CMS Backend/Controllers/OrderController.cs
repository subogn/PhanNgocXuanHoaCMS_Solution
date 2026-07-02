using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // 4. Khai báo biến kết nối Database
        private readonly ApplicationDbContext _context;

        // 5. Hàm khởi tạo (Constructor): "Tiêm" kết nối Database vào để sử dụng
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Chỉ định đây là phương thức GET (Dùng để lấy dữ liệu)
        [HttpGet]
        public IActionResult GetAll()
        {
            // Lấy dữ liệu từ bảng Orders
            var order = _context.Orders
             .Select(p => new
             {
                 p.Id,
                 p.OrderDate,
                 p.Status,
                 p.Notes,
                 CustomerName = p.Customer.FullName
             })
             .OrderByDescending(p => p.Id)
             .ToList();

            // Trả về kết quả cho Frontend kèm mã trạng thái 200 (Thành công)
            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInputDTO input)
        {
            if (input == null)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ"
                });
            }

            try
            {
                var order = new Order
                {
                    CustomerId = input.CustomerId,
                    OrderDate = DateTime.Now,
                    Status = 0,
                    Notes = input.Notes
                };

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                foreach (var item in input.Items)
                {
                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };

                    _context.OrderDetails.Add(detail);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Đặt hàng thành công",
                    orderId = order.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
    }

        // LỚP DTO TRUNG GIAN ĐỂ HỨNG DỮ LIỆU TỪ FRONTEND TRUYỀN LÊN
        public class OrderInputDTO
    {
        public int CustomerId { get; set; }

        public string? Notes { get; set; }

        public List<OrderDetailInputDTO> Items { get; set; }
    }

    public class OrderDetailInputDTO
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}

