using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {

            IEnumerable<Category> catList = await _unitOfWork.Category.GetAllAsync();

            ProductVM productVM = new ProductVM()
            {

                Product = new Product(),

                CategoryList = catList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
                
            };

            if(id == null)
            {
                return View(productVM);
            }

            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());

            if(productVM.Product == null)
            {
                return NotFound();
            }

            return View(productVM);

        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {

            IEnumerable<Category> catList = await _unitOfWork.Category.GetAllAsync();

            if (ModelState.IsValid)
            {

                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if(files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if(productVM.Product.ImageUrl != null)
                    {
                        // this is an edit and we need to remove old image
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using(var filesStreams = new FileStream(Path.Combine(uploads, fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }

                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;

                }
                else
                {

                    // update when they do not change the image
                    if(productVM.Product.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(productVM.Product.Id);

                        productVM.Product.ImageUrl = objFromDb.ImageUrl;

                    }

                }


                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            
            }
            else
            {
                productVM.CategoryList = catList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

                productVM.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }


            if(productVM.Product.Id != 0)
            {
                productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
            }

            return View(productVM);

        }
















        // API calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var objFromDb = _unitOfWork.Product.GetAll(includeProperies: "Category,CoverType");
            return Json(new { data = objFromDb });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _unitOfWork.Product.Get(id);

            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting the Product" });
            }



            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }




            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "You have removed the Product" });





        }





    }
}
