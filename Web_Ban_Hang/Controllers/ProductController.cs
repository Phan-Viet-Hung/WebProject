using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Web_Ban_Hang.Models;
using X.PagedList.Extensions;

namespace Web_Ban_Hang.Controllers
{
    public class ProductController : Controller
    {
        WebShopDbContext _context;
        public ProductController(WebShopDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetInt32("UserRole");

            // Lấy tên action đang thực thi
            var actionName = context.RouteData.Values["action"]?.ToString();

            // Kiểm tra nếu không phải action cần kiểm tra role
            if (actionName != "Index" && actionName != "Details") // Nếu là action Login và Register không cần kiểm tra role
            {
                // Kiểm tra nếu vai trò là null hoặc không hợp lệ
                if (role == null)
                {
                    context.Result = View("Error", "Shared");
                }
                else if (role == 1) // Nếu là admin
                {
                    var controllerName = context.RouteData.Values["controller"]?.ToString();
                    if (controllerName == "Product" && (actionName == "Index" || actionName == "Details" || actionName == "Create" || actionName == "Edit"))
                    {
                        // Cho phép admin truy cập vào các action cần thiết
                        base.OnActionExecuting(context);
                        return;
                    }

                    context.Result = RedirectToAction("Index", "Product");
                }

                else if (role == 0) // Nếu là khách
                {
                    context.Result = View("Error", "Shared");
                }
            }
        }
        // GET: ProductController
        public IActionResult Index(string name, int page = 1, int pageSize = 5)
        {
            // Tạo truy vấn ban đầu
            var query = _context.Products.AsQueryable();
            var userrole = HttpContext.Session.GetInt32("UserRole");
            if (userrole == 1)
            {
                TempData["UserRole"] = "Admin";
            }
            // Nếu có tham số tìm kiếm, áp dụng điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.ProductName.ToLower().Contains(name.ToLower())); // Tìm kiếm theo tên
            }

            // Tổng số bản ghi sau khi tìm kiếm (nếu có)
            var totalItems = query.Count();

            // Lấy dữ liệu cho trang hiện tại
            var pagedResult = query
                .OrderBy(u => u.ProductName) // Sắp xếp theo tên
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

        // GET: ProductController/Details/5
        public ActionResult Details(Guid id)
        {
            //var i = _context.Products.Include(x=>x.User).FirstOrDefault(x=>x.ProductId == id);
            //ViewData["role"] = i.User.Role;
            var i = _context.Products.Find(id);
            return View(i);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            Product pro = new Product()
            {
                ProductName = "Áo thun free size",
                Description = "Áo đẹp như người yêu bạn",
                Quantity = 10,
                Price = Convert.ToInt32(39000),
            };
            return View(pro);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product pro, IFormFile ImageFile)
        {
            try
            {
                // Gán ProductId nếu nó là Guid và không tự động sinh
                if (pro.ProductId == Guid.Empty)
                {
                    pro.ProductId = Guid.NewGuid();
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    pro.Image = "/images/" + fileName;
                }

                //if (!ModelState.IsValid)
                //{
                //    TempData["CreateProduct"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                //    return RedirectToAction("Index", "Product");
                //}

                _context.Products.Add(pro);
                _context.SaveChanges();

                TempData["CreateProduct"] = "Tạo sản phẩm thành công.";
                return RedirectToAction("Index", "Product");
            }
            catch (Exception e)
            {
                if (pro.Image != null)
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pro.Image.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                TempData["CreateProduct"] = "Tạo sản phẩm thất bại. Lỗi: " + e.Message;
                return RedirectToAction("Index", "Product");
            }
        }



        // GET: ProductController/Edit/5
        public ActionResult Edit(Guid id)
        {
            var editIem = _context.Products.FirstOrDefault(x => x.ProductId == id);
            return View(editIem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, Product pro, IFormFile ImageFile)
        {
            try
            {
                var existingProduct = _context.Products.FirstOrDefault(x => x.ProductId == id);

                if (existingProduct != null)
                {
                    // Cập nhật thông tin khác
                    existingProduct.ProductName = pro.ProductName;
                    existingProduct.Description = pro.Description;
                    existingProduct.Quantity = pro.Quantity;
                    existingProduct.Price = pro.Price;
                    existingProduct.Status = pro.Status;

                    // Kiểm tra xem có ảnh mới không
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu cần (nếu ảnh cũ không phải ảnh mặc định)
                        if (!string.IsNullOrEmpty(existingProduct.Image) && !existingProduct.Image.Contains("default.png"))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.Image.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Upload ảnh mới
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        string filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            ImageFile.CopyTo(stream);
                        }

                        // Cập nhật đường dẫn ảnh
                        existingProduct.Image = "/images/" + fileName;
                    }

                    _context.Products.Update(existingProduct);
                    _context.SaveChanges();

                    TempData["update"] = "Bạn đã cập nhật thông tin sản phẩm thành công.";
                }
                else
                {
                    TempData["update"] = "Không tìm thấy sản phẩm cần chỉnh sửa.";
                }

                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                TempData["update"] = "Cập nhật sản phẩm thất bại: " + ex.Message;
                return RedirectToAction("Index", "Product");
            }
        }



    }
}
