using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.Models.ViewModels;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperies: "Category,CoverType");

            // id logged in user 
            // claim null if user not logged in
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value)
                  .ToList().Count();

                // sum products display
                //var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Sum(s => s.Count);

                HttpContext.Session.SetInt32(StaticDetails.ssShoppingCart, count);
            }

            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperies: "Category,CoverType");

            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;

            if (ModelState.IsValid)
            {
                //Then add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                // storing id of logged in user
                cartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartObject.ApplicationUserId && u.ProductId == cartObject.ProductId,
                    includeProperies: "Product");

                if (cartFromDb == null)
                {
                    // no records exists in the database for that product for that user
                    _unitOfWork.ShoppingCart.Add(cartObject);
                }
                else
                {
                    cartFromDb.Count += cartObject.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }

                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == cartObject.ApplicationUserId)
                    .ToList().Count();


                HttpContext.Session.SetInt32(StaticDetails.ssShoppingCart, count);

                // extension session method - taking shoppingcart object storing in variable
                // obj will store the retrieved shopping cart

                // HttpContext.Session.SetObject(StaticDetails.ssShoppingCart, cartObject);
                // var obj = HttpContext.Session.GetObject<ShoppingCart>(StaticDetails.ssShoppingCart);

                return RedirectToAction(nameof(Index));

            }
            else
            {

                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == cartObject.ProductId, includeProperies: "Category, CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };

                return View(cartObject);

            }
        }












        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
