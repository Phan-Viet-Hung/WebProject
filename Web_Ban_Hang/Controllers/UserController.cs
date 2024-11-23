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

            // Lấy tên action đang thực thi
            var actionName = context.RouteData.Values["action"]?.ToString();

            // Kiểm tra nếu không phải action cần kiểm tra role
            if (actionName != "Login" && actionName != "Register" && actionName != "Logout") // Nếu là action Login và Register không cần kiểm tra role
            {
                // Kiểm tra nếu vai trò là null hoặc không hợp lệ
                if (role == null)
                {
                    context.Result = Content("Bạn không có quyền truy cập vào trang này.");
                }
                else if (role == 1) // Nếu là admin
                {
                    var controllerName = context.RouteData.Values["controller"]?.ToString();
                    if (controllerName != "User" || actionName != "Index")
                    {
                        context.Result = RedirectToAction("Index", "User");
                    }
                }
                else if (role == 0) // Nếu là khách
                {
                    context.Result = Content("Bạn không có quyền truy cập vào trang này");
                }
            }
            else
            {
                if(role == 1)
                {
                    context.Result = RedirectToAction("Index", "User");
                }
                else if(role == 0)
                {
                    context.Result = RedirectToAction("Index", "Home");
                }
            }

            base.OnActionExecuting(context);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }



        // GET: UserController
        public ActionResult Index(string name,int page, int pageSize)
        {
            
            var listAccount = _context.Users.ToPagedList(page,pageSize);
            if (string.IsNullOrEmpty(name))
            {
                return View(listAccount);
            }
            else
            {
                var search = _context.Users.Where(u => u.Name.ToLower().Contains(name.ToLower())).ToList();
                if(search.Count == 0)
                {
                    return View(listAccount);
                }
                else
                {
                    return View(search);
                }
            }
        }


        // GET: UserController/Details/5
        public ActionResult Details(Guid id)
        {
            var i = _context.Users.Find(id);
            return View(i);
        }

        // GET: UserController/Create
        public ActionResult Create()
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
        public ActionResult Create(User us)
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
                    UserName = us.Name,
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

            if (user != null)
            {
                // Lưu thông tin đăng nhập vào session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetInt32("UserRole", user.Role); // Lưu Role
                var userrole = HttpContext.Session.GetString("UserRole"); //lấy role


                // Điều hướng theo vai trò
                if (user.Role == 1) // Admin
                {
                    TempData["UserRole"] = userrole ?? "Admin";
                    return RedirectToAction("Index","User");
                }
                else // Khách hàng
                {
                    TempData["UserRole"] = userrole ?? "Quý khách";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid phone number or password.");
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
                TempData["update"] = "Bạn đã cập nhật thông tin thánh công";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                return View();
            }
        }
    }
}
