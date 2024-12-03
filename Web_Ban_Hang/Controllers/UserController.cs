using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Web_Ban_Hang.Models;
using X.PagedList.Extensions;

namespace Web_Ban_Hang.Controllers
{
    public class UserController : Controller
    {
        WebShopDbContext _context;
        public UserController( WebShopDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetInt32("UserRole");
            TempData["UserName"] = HttpContext.Session.GetString("UserName");
            

            // Lấy tên action đang thực thi
            var actionName = context.RouteData.Values["action"]?.ToString();

            // Kiểm tra nếu không phải action cần kiểm tra role
            if (actionName != "Login" && actionName != "Register") // Nếu là action Login và Register không cần kiểm tra role
            {
                // Kiểm tra nếu vai trò là null hoặc không hợp lệ
                if (role == null)
                {
                    context.Result = View("Error", "Shared");
                }
                else if (role == 1) // Nếu là admin
                {
                    var controllerName = context.RouteData.Values["controller"]?.ToString();
                    if (controllerName == "User" && (actionName == "Index" || actionName == "Details" || actionName == "Create" || actionName == "Edit"))
                    {
                        // Cho phép admin truy cập vào các action cần thiết
                        base.OnActionExecuting(context);
                        return;
                    }

                    context.Result = RedirectToAction("Index", "User");
                }

                else if (role == 0) // Nếu là khách
                {
                    context.Result = View("Error", "Shared");
                }
            }
            else
            {
                if(role == 1)
                {
                    TempData["UserRole"] = "Admin";
                    context.Result = RedirectToAction("Index", "User");
                }
                else if(role == 0)
                {
                    TempData["UserRole"] = "Khách";
                    context.Result = RedirectToAction("Index", "Home");
                }
            }

            base.OnActionExecuting(context);
        }



        // GET: UserController
        public ActionResult Index(string name, int page = 1, int pageSize = 5)
        {
            // Tạo truy vấn ban đầu
            var query = _context.Users.AsQueryable();
            var userrole = HttpContext.Session.GetInt32("UserRole");
            if (userrole == 1)
            {
                TempData["UserRole"] = "Admin";
            }
            // Nếu có tham số tìm kiếm, áp dụng điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Name.ToLower().Contains(name.ToLower())); // Tìm kiếm theo tên
            }

            // Tổng số bản ghi sau khi tìm kiếm (nếu có)
            var totalItems = query.Count();

            // Lấy dữ liệu cho trang hiện tại
            var pagedResult = query
                .OrderBy(u => u.Name) // Sắp xếp theo tên
                .Skip((page - 1) * pageSize) // Bỏ qua các bản ghi của trang trước
                .Take(pageSize) // Lấy số bản ghi của trang hiện tại
                .ToList();

            // Sử dụng ViewBag để truyền thêm thông tin vào View
            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = name;

            return View(pagedResult); // Trả danh sách người dùng đã phân trang
        }



        // GET: UserController/Details/5
        public IActionResult Details(Guid id)
        {
            var user = _context.Users
                .Include(u => u.CartDetails) // Include để lấy thông tin CartDetails nếu cần
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại."); // Trả về 404 nếu không tìm thấy
            }

            // Lấy CartId từ Cart nếu tồn tại
            var cartId = _context.Carts
                .Where(c => c.UserId == id)
                .Select(c => c.Id)
                .FirstOrDefault();

            ViewBag.CartId = cartId; // Gửi CartId qua ViewBag

            return View(user); // Truyền đối tượng user vào view
        }


        // GET: UserController/Create
        public IActionResult Create()
        {
            
                User u = new User()
                {
                    Name = "Phan Việt Hùng",
                    Email = "hungdepzai@gmail.com",
                    Password = "123456",
                    Age = 19
                };
                return View(u);

        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User us)
        {
            _context.Users.Add(us);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        //Đăng ký
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User us)
        {
            try
            {
                us.Role = 0; // Mặc định khách hàng
                _context.Users.Add(us);

                Cart cart = new Cart()
                {
                    Id = us.CartId,
                    UserId = us.Id
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //Đăng nhập
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string phoneNumber, string password)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Phone number and password are required.");
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber && u.Password == password);

            
            if (user != null && user.PhoneNumber == phoneNumber && user.Password == password)
            {                // Lưu thông tin đăng nhập vào session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetInt32("UserRole", user.Role); // Lưu Role
                var userrole = HttpContext.Session.GetString("UserRole"); //lấy role


                // Điều hướng theo vai trò
                if (user.Role == 1) // Admin
                {
                    // Sau khi người dùng đăng nhập thành công
                    HttpContext.Session.SetString("UserName", user.Name);
                    return RedirectToAction("Index","User");
                }
                else // Khách hàng
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["invalid"] = "Sai SĐT hoặc Password rồi, vui long xem lại bạn nhóe";
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(Guid id)
        {
            var i = _context.Users.FirstOrDefault(u => u.Id == id);
            return View(i);
        }
        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User us)
        {
            try
            {
                _context.Update(us);
                _context.SaveChanges();
                TempData["update"] = "Bạn đã cập nhật thông tin thành công";
                return RedirectToAction("Index","User");
            }
            catch(Exception e)
            {
                return View(e.Message);
            }
        }
    }
}
