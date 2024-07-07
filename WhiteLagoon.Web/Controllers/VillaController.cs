using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly ApplicationDbContext context;
        public VillaController(ApplicationDbContext context)
        {
            this.context = context;   
        }
        public IActionResult Index()
        {
            var villas = context.Villas.ToList();
            return View(villas);
        }

        public IActionResult Details(int id)
        {
            var villa = context.Villas.Find(id);
            return View(villa);
        }
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

        public IActionResult Create()
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa villa) {

            if(villa.Name == villa.Description)
            {
                ModelState.AddModelError("Name", "The name and description cannot match");
            }
            if (ModelState.IsValid)
            {
                context.Villas.Add(villa);
                context.SaveChanges();
                TempData["success"] = "Villa created successfully.";
                return RedirectToAction("Index", "Villa");
            }
            return View(villa);
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
