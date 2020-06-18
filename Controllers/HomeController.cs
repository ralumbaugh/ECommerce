using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Diagnostics;
using ECommerce.Models;
using System.Linq;
using System;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID == null)
            {
                return RedirectToAction("Index");
            }
            CommerceWrapper Wrapper = new CommerceWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(user => user.UserId == (int)LoggedInUserID);
            Wrapper.AllUsers = dbContext.Users.OrderByDescending(user => user.CreatedAt).Take(3).ToList();
            Wrapper.AllProducts = dbContext.Products.ToList();
            Wrapper.AllOrders = dbContext.Orders.OrderByDescending(order=> order.CreatedAt).Take(3).Include(User => User.User).Include(Order => Order.Product).ToList();
            return View("Dashboard", Wrapper);
        }
        [HttpPost("SeeMoreProducts")]
        public IActionResult SeeMoreProducts(CommerceWrapper Wrapper)
        {
            Wrapper.ThingsShown += 15;
            return Products(Wrapper.ThingsShown);
        }
        [HttpGet("products")]
        public IActionResult Products(int NumOfProducts = 15)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID == null)
            {
                return RedirectToAction("Index");
            }
            CommerceWrapper Wrapper = new CommerceWrapper();
            Wrapper.AllProducts = dbContext.Products.Take(NumOfProducts).ToList();
            return View("Products", Wrapper);
        }
        [HttpGet("orders")]
        public IActionResult Orders()
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID == null)
            {
                return RedirectToAction("Index");
            }
            CommerceWrapper Wrapper = new CommerceWrapper();
            Wrapper.AllProducts= dbContext.Products.ToList();
            Wrapper.AllOrders = dbContext.Orders.Include(product => product.Product).Include(user => user.User).ToList();
            return View("Orders", Wrapper);
        }
        [HttpGet("customers")]
        public IActionResult Customers()
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID == null)
            {
                return RedirectToAction("Index");
            }
            CommerceWrapper Wrapper= new CommerceWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(user => user.UserId == (int)LoggedInUserID);
            Wrapper.AllUsers = dbContext.Users.ToList();
            return View("Customers", Wrapper);
        }
        [HttpPost("NewProduct")]
        public IActionResult NewProduct(CommerceWrapper Wrapper)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID == null)
            {
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid)
            {
                dbContext.Products.Add(Wrapper.CurrentProduct);
                dbContext.SaveChanges();
                return RedirectToAction("Products");
            }
            return Products();
        }
        [HttpPost("NewOrder")]
        public IActionResult NewOrder(CommerceWrapper Wrapper)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID == null)
            {
                return RedirectToAction("Index");
            }
            Wrapper.CurrentOrder.UserId=(int)LoggedInUserID;
            Product PurchasedProduct = dbContext.Products.FirstOrDefault(product => product.ProductId == Wrapper.CurrentOrder.ProductId);
            if(PurchasedProduct == null)
            {
                ModelState.AddModelError("CurrentOrder.ProductId", "We don't have that item in stock.");
                return Orders();
            }
            if(Wrapper.CurrentOrder.Quantity > PurchasedProduct.Quantity)
            {
                ModelState.AddModelError("CurrentOrder.Quantity", $"Sorry, we only have {PurchasedProduct.Quantity} {PurchasedProduct.ItemName} available.");
            }
            if(ModelState.IsValid)
            {
                PurchasedProduct.Quantity -= Wrapper.CurrentOrder.Quantity;
                dbContext.Update(PurchasedProduct);
                dbContext.Entry(PurchasedProduct).Property("CreatedAt").IsModified=false;
                dbContext.Orders.Add(Wrapper.CurrentOrder);
                dbContext.SaveChanges();
                return RedirectToAction("Orders");
            }
            return Orders();
        }
        [HttpPost("DeleteUser")]
        public IActionResult DeleteUser(CommerceWrapper Wrapper)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID == null)
            {
                return RedirectToAction("Index");
            }
            int UserToDeleteID = Wrapper.ThingsShown;
            User CurrentUser = dbContext.Users.FirstOrDefault(user => user.UserId == (int)LoggedInUserID);
            User UserToDelete = dbContext.Users.FirstOrDefault(user => user.UserId == UserToDeleteID);
            if(CurrentUser.IsAdmin == true || CurrentUser.UserId == UserToDelete.UserId)
            {
                dbContext.Users.Remove(UserToDelete);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Customers");
        }
        [HttpPost("IndividualUser")]
        public IActionResult IndividualUser(CommerceWrapper Wrapper)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID == null)
            {
                return RedirectToAction("Index");
            }
            Wrapper.CurrentUser = dbContext.Users.Include(order => order.Orders).ThenInclude(item => item.Product).FirstOrDefault(user => user.UserId == Wrapper.ThingsShown);
            return View("IndividualUser", Wrapper);
        }
        public IActionResult Login(LoginWrapper WrappedUser)
        {
            LoginUser user = WrappedUser.LoginUser;
            if(ModelState.IsValid)
            {
                User UserInDb = dbContext.Users.FirstOrDefault(u=> u.Email == user.Email);
                if(UserInDb == null)
                {
                    ModelState.AddModelError("LoginUser.Email", "The email/password combination is incorrect.");
                    return View("Index");
                }
                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                var result = Hasher.VerifyHashedPassword(user, UserInDb.Password, user.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginUser.Email", "The email/password combination is incorrect.");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("LoggedInUserID", UserInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        public IActionResult Register(LoginWrapper WrappedUser)
        {
            User user = WrappedUser.NewUser;
            if(dbContext.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("NewUser.Email", "Email already in use!");
            }
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("LoggedInUserID", user.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
