using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
      //private readonly IVillaNumberRepository villaNumberRepository;
      //private readonly IVillaRepository villaRepository;
      private readonly IUnitOfWork unitOfWork;

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            
        }
        public IActionResult Index()
        {
            var villasNumbers = unitOfWork.VillaNumber.GetAll(null, "Villa");

            return View(villasNumbers);
        }



        public IActionResult Delete(int id)
        {
            VillaNumberViewModel obj = new()
            {
                VillaList = unitOfWork.VillaNumber.GetAll(null, "Villa").Select(vn => new SelectListItem
                {
                    Text = vn.Villa.Name,
                    Value = vn.Villa.Id.ToString()
                }),
                villaNumber = unitOfWork.VillaNumber.Get(u => u.Villa_Number == id)
            };



            if (obj.villaNumber is null)
            {
                return RedirectToAction("Error", "Home");

            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberViewModel obj)
        {
            // ModelState.Remove("Villa");

            if (ModelState.IsValid)
            {
                unitOfWork.VillaNumber.Remove(obj.villaNumber);
                unitOfWork.VillaNumber.Save();
                TempData["success"] = "Villa number deleted successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }

            obj.VillaList = unitOfWork.Villa.GetAll().Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
            return View(obj);

        }
        [HttpGet]
        public IActionResult Create()
        {
            VillaNumberViewModel VillaNumberVM = new()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                })
            };

            return View(VillaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberViewModel obj)
        {

            // ModelState.Remove("Villa");
            bool VillaNumberExists = unitOfWork.VillaNumber.GetAll(null, "Villa").Any(u => u.Villa_Number == obj.villaNumber.Villa_Number);

            if (ModelState.IsValid && !VillaNumberExists)
            {
                unitOfWork.VillaNumber.Add(obj.villaNumber);
                unitOfWork.VillaNumber.Save();
                TempData["success"] = "Villa number created successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }
            if (VillaNumberExists)
            {
                TempData["error"] = "This villa number exists already";
            }
            obj.VillaList = unitOfWork.VillaNumber.GetAll(null,"Villa").Select(v => new SelectListItem
            {
                Text = v.Villa.Name,
                Value = v.Villa.Id.ToString()
            });
            return View(obj);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            VillaNumberViewModel obj = new()
            {
                VillaList = unitOfWork.Villa.GetAll().Select(vn => new SelectListItem
                {
                    Text = vn.Name,
                    Value = vn.Id.ToString()
                }),
                villaNumber = unitOfWork.VillaNumber.Get(u => u.Villa_Number == id)
            };



            if (obj.villaNumber is null)
            {
                return RedirectToAction("Error", "Home");

            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberViewModel obj)
        {
            // ModelState.Remove("Villa");

            if (ModelState.IsValid)
            {
                unitOfWork.VillaNumber.Update(obj.villaNumber);
                unitOfWork.VillaNumber.Save();
                TempData["success"] = "Villa number updated successfully.";
                return RedirectToAction("Index", "VillaNumber");
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
