using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Web_Ban_Hang.Models;

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


        public IActionResult Index()
        {
            var userrole = HttpContext.Session.GetString("UserRole");
            TempData["UserRole"] = userrole ?? "Guest";
            // Lấy danh sách sản phẩm từ database
            var products = _context.Products
                .Where(p => p.Quantity > 0) // Chỉ lấy sản phẩm còn hàng
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Image = p.Image
                })
                .ToList();

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
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                TempData["notlogin"] = "Bạn chưa đăng nhập. Vui lòng đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }

            // Chuyển UserId từ chuỗi sang Guid
            Guid userGuid = Guid.Parse(userId);

            // Lấy giỏ hàng từ cơ sở dữ liệu

            var cartItems = _context.Carts
                .Where(c => c.UserId == userGuid)
                .Select(c => new
                {
                    c.Id,
                    c.UserName,
                    c.ProductId,
                    c.Quantity,
                    c.TotalAmount,
                    c.CreatedAt,
                    ProductName = _context.Products.FirstOrDefault(p => p.ProductId == c.ProductId).ProductName,
                    ProductImage = _context.Products.FirstOrDefault(p => p.ProductId == c.ProductId).Image,
                    ProductPrice = _context.Products.FirstOrDefault(p => p.ProductId == c.ProductId).Price
                })
                .ToList();

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
        //public IActionResult RemoveFromCart(Guid productId)
        //{

        //}
    }
}
