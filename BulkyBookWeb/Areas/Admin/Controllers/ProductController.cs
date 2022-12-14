using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BulkyBookWeb.Controllers;

    [Area("Admin")]
    
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
            
            
            //TempData["Success"] = "Everything Workds Great Jearb!@";
            return View();
        }
        
        
        

        public IActionResult Upsert(int? id)
        {
            
            ProductVM productVM = new ProductVM()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
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
            if (id == null || id == 0)
            {
                //create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                
                return View(productVM);
            }
            else
            {
                //update product
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                return View(productVM);
            }
           
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM model, IFormFile? file)
        {
            
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images/products/");
                    var extension = Path.GetExtension(file.FileName);
                    if(model.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, model.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    model.Product.ImageUrl = @"images/products/" + fileName + extension;
                }
                if(model.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(model.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(model.Product);
                }
                
                _unitOfWork.Save();
                TempData["Success"] = "Product Created Successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //public IActionResult Delete(int id)
        //{
        //    if (id == null || id <= 0)
        //    {
        //        return NotFound();
        //    }
        //    Product productToDelete = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
        //    if (productToDelete == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productToDelete);
        //}
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult Delete(Product productToDelete)
        //{
        //    _unitOfWork.Product.Remove(productToDelete);
        //    _unitOfWork.Save();
        //    TempData["Success"] = "Cover Type Deleted Successfully!";
        //    return RedirectToAction("Index");
        //}
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = productList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var model = _unitOfWork.Product.GetFirstOrDefault(m=>m.Id==id);
            if (model == null)
            {
                return Json(new { success = false, message = "Not Found...." });
            }
            
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, model.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            
            _unitOfWork.Product.Remove(model);
            _unitOfWork.Save();
            //TempData["success"] = "Product deleted successfully";
            return Json(new {success = true, message = "Product Deleted Successfully!"});
        }
        #endregion
    }



