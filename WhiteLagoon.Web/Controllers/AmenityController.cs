using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;

namespace WhiteLagoon.Web.Controllers
{
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public AmenityController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var amenities = unitOfWork.Amenity.GetAll();
            
            return View(amenities);
        }
    }
}
