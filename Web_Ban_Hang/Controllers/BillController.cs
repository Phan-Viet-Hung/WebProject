using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Controllers
{
    public class BillController : Controller
    {
        WebShopDbContext _context;
        public BillController(WebShopDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetInt32("UserRole");

            // Lấy tên action đang thực thi
            var actionName = context.RouteData.Values["action"]?.ToString();

            // Kiểm tra nếu không phải action cần kiểm tra role
            if (actionName !=  "BillDetails" && actionName != "Payment") // Nếu là action Login và Register không cần kiểm tra role
            {
                // Kiểm tra nếu vai trò là null hoặc không hợp lệ
                if (role == null)
                {
                    context.Result = Content("Bạn không có quyền truy cập vào trang này.");
                }
                else if (role == 1) // Nếu là admin
                {
                    var controllerName = context.RouteData.Values["controller"]?.ToString();
                    if (controllerName == "Bill" && (actionName == "Index"))
                    {
                        // Cho phép admin truy cập vào các action cần thiết
                        base.OnActionExecuting(context);
                        return;
                    }

                    context.Result = RedirectToAction("Index", "Bill");
                }

                else if (role == 0) // Nếu là khách
                {
                    context.Result = Content("Bạn không có quyền truy cập vào trang này");
                }
            }
        }
        // GET: BillController
        public ActionResult Index(string search, int page = 1, int pageSize = 5)
        {
            var query = _context.Bill.Include(b => b.User).AsQueryable(); // Giả sử bạn có quan hệ với User trong Bill

            // Nếu có tham số tìm kiếm, áp dụng điều kiện lọc cho cả tên người dùng, số điện thoại và trạng thái
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower(); // Chuyển tìm kiếm về chữ thường để so sánh dễ dàng hơn
                query = query.Where(b => b.User.Name.ToLower().Contains(search) ||
                                          b.User.PhoneNumber.Contains(search) ||
                                          b.Status.ToLower().Contains(search));
            }

            // Tổng số bản ghi sau khi tìm kiếm (nếu có)
            var totalItems = query.Count();

            // Lấy dữ liệu cho trang hiện tại
            var pagedResult = query
                .OrderBy(b => b.CreateDate) // Sắp xếp theo ngày tạo hóa đơn
                .Skip((page - 1) * pageSize) // Bỏ qua các bản ghi của trang trước
                .Take(pageSize) // Lấy số bản ghi của trang hiện tại
                .ToList();

            // Sử dụng ViewBag để truyền thêm thông tin vào View
            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = search; // Truyền từ khóa tìm kiếm vào ViewBag

            return View(pagedResult); // Trả danh sách hóa đơn đã phân trang
        }



        // GET: BillController/Details/5
        public IActionResult BillDetails(Guid id)
        {
            var bill = _context.Bill
                .Include(b => b.Details)
                .ThenInclude(d => d.Products)
                .FirstOrDefault(b => b.Id == id);

            if (bill == null)
            {
                return NotFound("Không tìm thấy hóa đơn.");
            }

            return View(bill);
        }
        public IActionResult Payment(Guid id)
        {
            // Lấy UserId từ session
            var userId = HttpContext.Session.GetString("UserId");

            // Kiểm tra nếu không có UserId trong session (nếu người dùng chưa đăng nhập)
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User"); // Redirect đến trang đăng nhập nếu chưa đăng nhập
            }

            // Chuyển UserId từ string sang Guid
            var userGuid = Guid.Parse(userId);

            // Lấy hóa đơn của người dùng từ cơ sở dữ liệu
            var bill = _context.Bill
                .Include(b => b.Details)
                .ThenInclude(d => d.Products)
                .FirstOrDefault(b => b.Id == id && b.UserId == userGuid); // Lọc theo UserId để đảm bảo người dùng không xem hóa đơn của người khác

            // Kiểm tra nếu hóa đơn không tồn tại
            if (bill == null)
            {
                return NotFound("Không tìm thấy hóa đơn.");
            }

            // Truyền thông tin hóa đơn vào ViewBag để hiển thị
            ViewBag.TotalAmount = bill.TotalAmount;

            // Truyền phương thức thanh toán vào View (có thể là một danh sách các phương thức)
            var paymentMethods = new List<string> { "Thẻ tín dụng", "Chuyển khoản ngân hàng", "Tiền mặt" };
            ViewBag.PaymentMethods = paymentMethods;

            return View(bill);
        }

    }
}
