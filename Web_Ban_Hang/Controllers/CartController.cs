using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Controllers
{
    public class CartController : Controller
    {
        WebShopDbContext _context;
        public CartController(WebShopDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetInt32("UserRole");

            // Lấy tên action đang thực thi
            var actionName = context.RouteData.Values["action"]?.ToString();

            if (actionName != "Details") 
            {
                // Kiểm tra nếu vai trò là null hoặc không hợp lệ
                if (role == null)
                {
                    context.Result = View("Error", "Shared");
                }
                else if (role == 1) // Nếu là admin
                {
                    var controllerName = context.RouteData.Values["controller"]?.ToString();
                    if (controllerName == "Cart" && (actionName == "Index"))
                    {
                        // Cho phép admin truy cập vào các action cần thiết
                        base.OnActionExecuting(context);
                        return;
                    }

                    context.Result = RedirectToAction("Index", "Cart");
                }

                else if (role == 0) // Nếu là khách
                {
                    context.Result = View("Error", "Shared");
                }
            }
        }

        // GET: Cart
        public ActionResult Index(string search, int page = 1, int pageSize = 5)
        {
            var query = _context.Carts
                .Join(_context.Users,
                      cart => cart.UserId,
                      user => user.Id,
                      (cart, user) => new
                      {
                          cart.Id,
                          cart.ProductId,
                          cart.Quantity,
                          cart.TotalAmount,
                          cart.CreatedAt,
                          UserName = user.Name // Lấy tên người dùng từ bảng Users
                      })
                .AsQueryable(); // Chuyển sang IQueryable để dễ dàng xử lý phân trang

            // Nếu có tham số tìm kiếm, áp dụng điều kiện lọc theo tên người dùng
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower(); // Chuyển tìm kiếm về chữ thường để so sánh dễ dàng hơn
                query = query.Where(c => c.UserName.ToLower().Contains(search));
            }

            // Tổng số bản ghi sau khi tìm kiếm (nếu có)
            var totalItems = query.Count();

            // Tổng số trang (tính toán số trang từ tổng số bản ghi)
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Lấy dữ liệu cho trang hiện tại
            var pagedResult = query
                .OrderBy(c => c.CreatedAt) // Sắp xếp theo ngày tạo giỏ hàng
                .Skip((page - 1) * pageSize) // Bỏ qua các bản ghi của trang trước
                .Take(pageSize) // Lấy số bản ghi của trang hiện tại
                .ToList();

            // Truyền thông tin phân trang và từ khóa tìm kiếm vào ViewBag
            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = search;

            return View(pagedResult); // Trả về danh sách giỏ hàng đã phân trang
        }


        // GET: Cart/Details/5
        public ActionResult Details(Guid id)
        {
            // Tìm giỏ hàng theo Id và kết hợp thông tin người dùng
            var cart = _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefault(c=>c.Id == id);


            // Kiểm tra nếu không tìm thấy giỏ hàng
            if (cart == null)
            {
                // Nếu không tìm thấy, trả về thông báo lỗi hoặc trang không tìm thấy
                return NotFound();
            }
            var userId = _context.Users
                .Where(u => u.CartId == id)
                .Select(u => u.Id);
            ViewBag.UserId = userId;

            // Truyền dữ liệu cho view (giỏ hàng và thông tin người dùng)
            return View(cart);
        }


    }
}
