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

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var villa = context.Villas.Find(id);
            if(villa != null)
            {
                context.Villas.Remove(villa);

            }
            

            return RedirectToAction("Index");
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
                return RedirectToAction("Index", "Villa");
            }
            return View(villa);
        }
    }
}
