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

        //public IActionResult Details(int id)
        //{
        //    var villa = context.Villas.Find(id);
        //    return View(villa);
        //}
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var villa = context.Villas.Find(id);
            if (villa is null)
            {
                return NotFound();

            }
             return View(villa);
        }
        [HttpPost]
        public IActionResult Delete(Villa villa)
        {   
            var villaFromDb = context.Villas.Find(villa.Id);
            if (villaFromDb is not null)
            {
                context.Villas.Remove(villaFromDb);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return NotFound();
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
                TempData["success"] = "Villa created successfully.";
                return RedirectToAction("Index", "VillaNumber");
            }
            if (VillaNumberExists)
            {
                TempData["error"] = "This villa number exists for this Villa";
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
            var villa = context.Villas.Find(id);
            if (villa is null)
            {
                return RedirectToAction("Error", "Home");

            }
            return View(villa);
        }

        [HttpPost]
        public IActionResult Update(Villa villa)
        {
            if (villa.Name == villa.Description)
            {
                ModelState.AddModelError("Name", "The name and description cannot match");
            }
            if (ModelState.IsValid && villa is not null)
            {
                context.Villas.Update(villa);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
