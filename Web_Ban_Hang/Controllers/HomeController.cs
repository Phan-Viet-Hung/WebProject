using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Web_Ban_Hang.Models;
using X.PagedList.Extensions;

namespace Web_Ban_Hang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebShopDbContext _context;

        public HomeController(ILogger<HomeController> logger, WebShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            if (HttpContext.Session.ToString() == null)
            {
                TempData["Logout"] = "Bạn đã đăng xuất thành công";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["MessLogout"] = "Đăng xuất thất bại, bạn chưa đăng nhập à";
                return RedirectToAction("Index", "Home");
            }
        }


        public IActionResult Index(string name, int? page)
        {
            TempData["UserName"] = HttpContext.Session.GetString("UserName");
            // Lấy vai trò người dùng từ Session
            var userrole = HttpContext.Session.GetInt32("UserRole");
            if (userrole == 1)
            {
                TempData["UserRole"] = "Admin";
            }
            else if (userrole == null)
            {
                TempData["UserRole"] = "Guess";
            }
            else
            {
                TempData["UserRole"] = "Khách";
            }


            // Số sản phẩm trên mỗi trang
            int pageSize = 6; // 2 dòng x 3 sản phẩm mỗi dòng
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            // Tạo truy vấn ban đầu cho sản phẩm
            var query = _context.Products
                                .Where(p => p.Quantity > 0); // Chỉ lấy sản phẩm còn hàng

            // Nếu có tham số tìm kiếm, áp dụng điều kiện lọc theo tên sản phẩm
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.ProductName.ToLower().Contains(name.ToLower()));
            }

            // Lấy danh sách sản phẩm đã phân trang
            var products = query
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Image = p.Image,
                    Quantity = p.Quantity // Bao gồm cả số lượng sản phẩm
                })
                .ToList()
                .ToPagedList(pageNumber, pageSize); // Thực hiện phân trang

            // Truyền thông tin phân trang và tìm kiếm vào ViewBag
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = query.Count();
            ViewBag.SearchTerm = name;

            return View(products);
        }


        public ActionResult Details(Guid id)
        {
            var i = _context.Products.Find(id);
            return View(i);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Cart()
        {
            // Lấy UserId từ session
            var userId = HttpContext.Session.GetString("UserId");
            //var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["notlogin"] = "Bạn chưa đăng nhập. Vui lòng đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }

            // Chuyển UserId từ chuỗi sang Guid
            Guid userGuid = Guid.Parse(userId);

            // Lấy giỏ hàng từ cơ sở dữ liệu
            var cartItems = (from c in _context.Carts
                             join p in _context.Products on c.ProductId equals p.ProductId into productGroup
                             from product in productGroup.DefaultIfEmpty() // Xử lý null
                             where c.UserId == userGuid
                             select new
                             {
                                 c.Id,
                                 c.ProductId,
                                 c.Quantity,
                                 c.TotalAmount,
                                 c.CreatedAt,
                                 ProductName = product != null ? product.ProductName : "Sản phẩm không tồn tại",
                                 ProductImage = product != null ? product.Image : "/images/default.png",
                                 ProductPrice = product != null ? product.Price : 0
                             }).ToList();


            // Kiểm tra nếu giỏ hàng trống

            foreach(var i in cartItems)
            {
                if (i.ProductId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    TempData["emptyCart"] = "Giỏ hàng của bạn đang trống.";
                    return View(new List<dynamic>()); // Hiển thị view giỏ hàng trống
                }
            }
            


            // Truyền dữ liệu vào View
            return View(cartItems);
        }
        [HttpPost]
        public IActionResult AddToCart(Guid id, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["notlogin"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Login", "User");
            }

            Guid userGuid = Guid.Parse(userId);

            var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == userGuid && c.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.TotalAmount = cartItem.Quantity * _context.Products.FirstOrDefault(p => p.ProductId == id).Price;
            }
            else
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    TempData["invalid"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("Index", "Home");
                }

                var newCartItem = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userGuid,
                    ProductId = id,
                    Quantity = quantity,
                    TotalAmount = quantity * product.Price,
                    CreatedAt = DateTime.Now
                };

                _context.Carts.Add(newCartItem);
            }

            _context.SaveChanges();

            TempData["addtocart"] = "Sản phẩm đã được thêm vào giỏ hàng.";
            return RedirectToAction("Cart", "Home");
        }
        public IActionResult RemoveFromCart(Guid cartId, Guid productId)
        {
            try
            {
                // Tìm sản phẩm trong giỏ hàng theo cartId và productId
                var cartItem = _context.Carts
                    .FirstOrDefault(x => x.Id == cartId && x.ProductId == productId);

                if (cartItem == null)
                {
                    TempData["deleteItem"] = "Sản phẩm không tồn tại trong giỏ hàng.";
                    return RedirectToAction("Cart", "Home");
                }

                // Xóa sản phẩm khỏi giỏ hàng
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();

                TempData["deleteItem"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc thông báo lỗi
                TempData["deleteItem"] = "Đã xảy ra lỗi khi xóa sản phẩm khỏi giỏ hàng. Vui lòng thử lại.";
            }

            return RedirectToAction("Cart", "Home");
        }


        [HttpPost]
        public IActionResult IncreaseQuantity(Guid id, Guid cartId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.Id == cartId && c.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                cartItem.TotalAmount = cartItem.Quantity * _context.Products.FirstOrDefault(p => p.ProductId == id)?.Price ?? 0;
                _context.SaveChanges();
            }
            return RedirectToAction("Cart"); // Redirect về trang giỏ hàng
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(Guid id, Guid cartId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.Id == cartId && c.ProductId == id);
            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                cartItem.TotalAmount = cartItem.Quantity * _context.Products.FirstOrDefault(p => p.ProductId == id)?.Price ?? 0;
                _context.SaveChanges();
            }
            return RedirectToAction("Cart"); // Redirect về trang giỏ hàng
        }

        public IActionResult Checkout()
        {
            try
            {
                // Lấy UserId hiện tại (có thể từ session hoặc context)
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Message"] = "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Cart");
                }

                Guid userGuid = Guid.Parse(userId);

                // Lấy danh sách sản phẩm trong giỏ hàng bằng LINQ
                var cartItems = (from c in _context.Carts
                                 join p in _context.Products on c.ProductId equals p.ProductId into productGroup
                                 from product in productGroup.DefaultIfEmpty() // Xử lý trường hợp không có sản phẩm
                                 where c.UserId == userGuid
                                 select new
                                 {
                                     c.Id,
                                     c.ProductId,
                                     c.Quantity,
                                     c.TotalAmount,
                                     c.CreatedAt,
                                     ProductName = product != null ? product.ProductName : "Sản phẩm không tồn tại",
                                     ProductImage = product != null ? product.Image : "/images/default.png",
                                     ProductPrice = product != null ? product.Price : 0
                                 }).ToList();

                // Nếu giỏ hàng rỗng
                if (!cartItems.Any())
                {
                    TempData["Message"] = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Cart");
                }

                // Tính tổng tiền hóa đơn
                decimal totalAmount = cartItems.Sum(item => item.TotalAmount);

                // Tạo hóa đơn mới
                var newBill = new Bill
                {
                    Id = Guid.NewGuid(),
                    UserId = userGuid,
                    CreateDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    Status = "Pending", // Mặc định trạng thái hóa đơn là "Pending"
                    Details = new List<BillDetail>()
                };

                // Tạo chi tiết hóa đơn từ giỏ hàng bằng LINQ
                var billDetails = (from c in _context.Carts
                                   join p in _context.Products on c.ProductId equals p.ProductId into productGroup
                                   from product in productGroup.DefaultIfEmpty()
                                   where c.UserId == userGuid
                                   select new BillDetail
                                   {
                                       Id = Guid.NewGuid(),
                                       BillId = newBill.Id,
                                       ProductId = c.ProductId,
                                       Quantity = c.Quantity,
                                       UnitPrice = product != null ? product.Price : 0, // Lấy giá sản phẩm từ bảng Products
                                   }).ToList();

                // Thêm danh sách chi tiết hóa đơn vào hóa đơn
                newBill.Details.AddRange(billDetails);


                // Lưu hóa đơn vào cơ sở dữ liệu
                _context.Bill.Add(newBill);

                // Xóa toàn bộ sản phẩm khỏi giỏ hàng sau khi tạo hóa đơn
                var cartToRemove = _context.Carts.Where(c => c.UserId == userGuid).ToList();
                _context.Carts.RemoveRange(cartToRemove);

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                // Truyền thông báo và điều hướng đến trang hóa đơn chi tiết
                TempData["bill"] = "Đặt hàng thành công. Hóa đơn của bạn đã được tạo.";
                return RedirectToAction("BillDetails", new { id = newBill.Id });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần) và thông báo lỗi
                TempData["bill"] = "Đã xảy ra lỗi khi đặt hàng. Vui lòng thử lại.";
                return RedirectToAction("Cart");
            }
        }
        public IActionResult UserBills()
        {
            // Lấy UserId từ session
            var userId = HttpContext.Session.GetString("UserId");

            // Kiểm tra nếu không có UserId trong session (nếu người dùng chưa đăng nhập)
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User"); // Redirect đến trang đăng nhập nếu chưa đăng nhập
            }

            // Chuyển UserId từ string sang Guid (giả sử UserId là Guid)
            var userGuid = Guid.Parse(userId);

            // Lấy danh sách hóa đơn của người dùng từ cơ sở dữ liệu
            var bills = _context.Bill
                .Where(b => b.UserId == userGuid) // Lọc theo UserId
                .Include(b => b.Details) // Nếu bạn muốn hiển thị chi tiết hóa đơn
                .ThenInclude(d => d.Products) // Bao gồm thông tin sản phẩm trong mỗi chi tiết hóa đơn
                .ToList();

            // Trả về view với danh sách hóa đơn
            return View(bills);
        }

        public IActionResult BillDetails(Guid id)
        {
            // Lấy UserId từ session
            var userId = HttpContext.Session.GetString("UserId");

            // Kiểm tra nếu không có UserId trong session (nếu người dùng chưa đăng nhập)
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User"); // Redirect đến trang đăng nhập nếu chưa đăng nhập
            }

            // Tìm người dùng trong cơ sở dữ liệu dựa trên UserId để lấy Role
            var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            // Kiểm tra nếu không tìm thấy người dùng
            if (user == null)
            {
                return RedirectToAction("Login", "User"); // Redirect đến trang đăng nhập nếu không tìm thấy người dùng
            }

            // Lấy Role của người dùng
            int role = user.Role; // Role có thể là 0 (user) hoặc 1 (admin)

            // Lấy thông tin hóa đơn từ cơ sở dữ liệu
            var bill = _context.Bill
                .Include(b => b.Details)
                .ThenInclude(d => d.Products)
                .FirstOrDefault(b => b.Id == id);

            // Kiểm tra nếu hóa đơn không tồn tại
            if (bill == null)
            {
                return NotFound("Không tìm thấy hóa đơn.");
            }

            // Truyền thông tin hóa đơn và role vào view
            ViewBag.UserRole = role; // Truyền vai trò người dùng vào ViewBag

            return View(bill);
        }
    }
}
