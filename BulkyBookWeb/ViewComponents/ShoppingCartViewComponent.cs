using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace BulkyBookWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // 2100399113479395 Facebook App Id
            // 365c574d5931772c3d2df60d3d981eb9 Facebook Secret

            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    
                    return View(HttpContext.Session.GetInt32(SD.SessionCart)); ;
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                        .GetAll(c => c.ApplicationUserId == claim.Value)
                        .ToList().Count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart)); ;
                }
            }
            
            
            HttpContext.Session.Clear();
            return View(0);
            
        }
    }
}
