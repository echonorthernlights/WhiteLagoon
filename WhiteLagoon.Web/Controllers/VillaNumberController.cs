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
       private readonly IVillaNumberRepository villaNumberRepository;
       private readonly IVillaRepository villaRepository;

        public VillaNumberController(IVillaNumberRepository villaNumberRepository, IVillaRepository villaRepository)
        {
            this.villaNumberRepository = villaNumberRepository;
            this.villaRepository = villaRepository;
            
        }
        public IActionResult Index()
        {
            var villasNumbers = villaNumberRepository.GetAll(null, "Villa");

            return View(villasNumbers);
        }



        public IActionResult Delete(int id)
        {
            VillaNumberViewModel obj = new()
            {
                VillaList = villaNumberRepository.GetAll(null, "Villa").Select(vn => new SelectListItem
                {
                    Text = vn.Villa.Name,
                    Value = vn.Villa.Id.ToString()
                }),
                villaNumber = villaNumberRepository.Get(u => u.Villa_Number == id)
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
                villaNumberRepository.Remove(obj.villaNumber);
                villaNumberRepository.Save();
                TempData["success"] = "Villa number deleted successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }

            obj.VillaList = villaRepository.GetAll().Select(v => new SelectListItem
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
                VillaList = villaRepository.GetAll().Select(v => new SelectListItem
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
            bool VillaNumberExists = villaNumberRepository.GetAll(null, "Villa").Any(u => u.Villa_Number == obj.villaNumber.Villa_Number);

            if (ModelState.IsValid && !VillaNumberExists)
            {
                villaNumberRepository.Add(obj.villaNumber);
                villaNumberRepository.Save();
                TempData["success"] = "Villa number created successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }
            if (VillaNumberExists)
            {
                TempData["error"] = "This villa number exists already";
            }
            obj.VillaList = villaNumberRepository.GetAll(null,"Villa").Select(v => new SelectListItem
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
                VillaList = villaRepository.GetAll().Select(vn => new SelectListItem
                {
                    Text = vn.Name,
                    Value = vn.Id.ToString()
                }),
                villaNumber = villaNumberRepository.Get(u => u.Villa_Number == id)
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
                villaNumberRepository.Update(obj.villaNumber);
                villaNumberRepository.Save();
                TempData["success"] = "Villa number updated successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }

            obj.VillaList = villaRepository.GetAll().Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
            return View(obj);
        }

    }
    }
