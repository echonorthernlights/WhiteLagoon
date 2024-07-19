using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = SD.Admin)]
    
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public AmenityController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var amenities = unitOfWork.Amenity.GetAll(includeProperties: "Villa");
            
            return View(amenities);
        }

        [HttpGet]
        public IActionResult Create()
        {
            AmenityViewModel AmenityVM = new()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                })
            };

            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityViewModel obj)
        {

            // ModelState.Remove("Villa");
            
            if (ModelState.IsValid )
            {
                unitOfWork.Amenity.Add(obj.Amenity);
                unitOfWork.Amenity.Save();
                TempData["success"] = "Amenity created successfully.";
                return RedirectToAction("Index", "Amenity");
            }
          
            obj.VillaList = unitOfWork.Villa.GetAll().Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
            return View(obj);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            AmenityViewModel obj = new()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(vn => new SelectListItem
                {
                    Text = vn.Name,
                    Value = vn.Id.ToString()
                }),
                Amenity = unitOfWork.Amenity.Get(u => u.Id == id)
            };



            if (obj.Amenity is null)
            {
                return RedirectToAction("Error", "Home");

            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(AmenityViewModel obj)
        {
            // ModelState.Remove("Villa");

            if (ModelState.IsValid)
            {
                unitOfWork.Amenity.Update(obj.Amenity);
                unitOfWork.VillaNumber.Save();
                TempData["success"] = "Villa amenity updated successfully.";
                return RedirectToAction("Index", "Amenity");
            }

            obj.VillaList = unitOfWork.Villa.GetAll().Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
            return View(obj);
        }

        public IActionResult Delete(int id)
        {
            AmenityViewModel obj = new()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(vn => new SelectListItem
                {
                    Text = vn.Name,
                    Value = vn.Id.ToString()
                }),
                Amenity = unitOfWork.Amenity.Get(u => u.Id == id)
            };



            if (obj.Amenity is null)
            {
                return RedirectToAction("Error", "Home");

            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(AmenityViewModel obj)
        {
            // ModelState.Remove("Villa");
            var amentiryFromDb = unitOfWork.Amenity.Get(a=>a.Id == obj.Amenity.Id);
            if (amentiryFromDb is not null)
            {
                unitOfWork.Amenity.Remove(amentiryFromDb);
                unitOfWork.Amenity.Save();
                TempData["success"] = "Villa amenity deleted successfully.";
                return RedirectToAction("Index", "Amenity");
            }

            obj.VillaList = unitOfWork.Villa.GetAll().Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
            return View(obj);

        }


    }
}
