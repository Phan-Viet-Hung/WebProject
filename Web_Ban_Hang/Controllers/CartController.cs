using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        // GET: CartController
        public ActionResult Index()
        {
            var cart = _context.Carts.ToList();
            return View(cart);
        }

        // GET: CartController/Details/5
        public ActionResult Details(Guid id)
        {
            var i = _context.Carts.Find(id);
            return View(i);
        }
    }
}
