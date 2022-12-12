using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var companies = _unitOfWork.Company.GetAll();
            TempData["success"] = "Company index page yay!";
            return View(companies);
        }
        public IActionResult Upsert(int? id)
        {
            Company company = new();
            if(id != null || id > 0)
            {
                company = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
                return View(company);
            }
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if(company.Id == 0)
            {
                _unitOfWork.Company.Add(company);
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully!";
            }
            else
            {
                _unitOfWork.Company.Update(company);
                _unitOfWork.Save();
                TempData["success"] = "Company updated successfully!";
            }
            return RedirectToAction("Index");
        }
        //public IActionResult Create() 
        //{
        //    TempData["success"] = "Welcome to Create";
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Company company)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Add(company);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Company Created Successfully!";
        //        return RedirectToAction("Index");
        //    }
        //    return View(company);
        //}
        
        //public IActionResult Edit(int? id)
        //{
        //    var company = _unitOfWork.Company.GetAll().FirstOrDefault(c=>c.Id==id);
        //    TempData["success"] = "Welcome to Edit Company!";
        //    return View(company);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(Company company)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Update(company);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Company Updated Successfully!";
        //        return RedirectToAction("Index");
        //    }
            
        //    return View(company);
        //}
        #region API CALLS
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.Company.GetAll();
            return Json(new{ data = companies});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToDelete = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
            if(companyToDelete == null)
            {
                return Json(new { success= false, message = "Not Found..." });
            }
            _unitOfWork.Company.Remove(companyToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company deleted successfully!" });
        }
        #endregion
    }
}
