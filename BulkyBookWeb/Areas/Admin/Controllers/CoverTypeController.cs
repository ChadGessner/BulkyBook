using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> coverTypeList = _unitOfWork.CoverType.GetAll();
            TempData["Success"] = "Everything Workds Great Jearb!@";
            return View(coverTypeList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["Success"] = "Cover Type Created Successfully!";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("Error", "Not a valid cover type or somethign...");
            return View(coverType);
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id <= 0)
            {
                return NotFound();
            }
            CoverType cover = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            return View(cover);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType cover)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(cover);
                _unitOfWork.Save();
                TempData["Success"] = "Cover Type Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View(cover);
        }

        public IActionResult Delete(int? id)
        {
            if(id == null || id <= 0)
            {
                return NotFound();
            }
            CoverType coverToDelete = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if(coverToDelete == null)
            {
                return NotFound();
            }
            return View(coverToDelete);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(CoverType coverToDelete)
        {
            _unitOfWork.CoverType.Remove(coverToDelete);
            _unitOfWork.Save();
            TempData["Success"] = "Cover Type Deleted Successfully!";
            return RedirectToAction("Index");
        }
    }
}
