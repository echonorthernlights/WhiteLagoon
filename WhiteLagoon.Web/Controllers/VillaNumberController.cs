using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext context;
        public VillaNumberController(ApplicationDbContext context)
        {
            this.context = context;   
        }

        public IActionResult Index()
        {
            var villasNumbers = context.VillaNumbers.Include(v=>v.Villa).ToList();
            return View(villasNumbers);
        }



        public IActionResult Delete(int id)
        {
        VillaNumberViewModel obj = new()
            {
                VillaList = context.Villas.ToList().Select(vn => new SelectListItem
                {
                    Text = vn.Name,
                    Value = vn.Id.ToString()
                }),
                villaNumber = context.VillaNumbers.FirstOrDefault(u => u.Villa_Number == id)
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
                context.VillaNumbers.Remove(obj.villaNumber);
                context.SaveChanges();
                TempData["success"] = "Villa number deleted successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }

            obj.VillaList = context.Villas.ToList().Select(v => new SelectListItem
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
                VillaList = context.Villas.ToList().Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                })
            };
    
            return View(VillaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberViewModel obj) {

            // ModelState.Remove("Villa");
            bool VillaNumberExists = context.VillaNumbers.Any(u =>  u.Villa_Number == obj.villaNumber.Villa_Number);
            
            if (ModelState.IsValid && !VillaNumberExists)
            {
                context.VillaNumbers.Add(obj.villaNumber);
                context.SaveChanges();
                TempData["success"] = "Villa number created successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }
            if (VillaNumberExists)
            {
                TempData["error"] = "This villa number exists already";
            }
            obj.VillaList = context.Villas.ToList().Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
            return View(obj);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            VillaNumberViewModel obj = new()
            {
                VillaList = context.Villas.ToList().Select(vn => new SelectListItem
                {
                    Text=vn.Name,
                    Value = vn.Id.ToString()
                }),
               villaNumber = context.VillaNumbers.FirstOrDefault(u => u.Villa_Number == id)
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
           
            if (ModelState.IsValid )
            {
                context.VillaNumbers.Update(obj.villaNumber);
                context.SaveChanges();
                TempData["success"] = "Villa number updated successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }

            obj.VillaList = context.Villas.ToList().Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
            return View(obj);
        }

    }
}
