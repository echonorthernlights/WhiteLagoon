using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaRepository villaRepository;
        public VillaController(IVillaRepository villaRepository)
        {
            this.villaRepository = villaRepository;   
        }
        public IActionResult Index()
        {
            var villas = villaRepository.GetAll();
            return View(villas);
        }

        public IActionResult Details(int id)
        {
            var villa = villaRepository.Get(v => v.Id == id);
            return View(villa);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var villa = villaRepository.Get(v => v.Id == id);
            villaRepository.Remove(villa);
            if (villa is null)
            {
                return NotFound();

            }
             return View(villa);
        }
        [HttpPost]
        public IActionResult Delete(Villa villa)
        {   
            var villaFromDb  = villaRepository.Get(v => v.Id == villa.Id);
            if (villaFromDb is not null)
            {
                villaRepository.Remove(villaFromDb);
                villaRepository.Save();
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
                villaRepository.Add(villa);
                villaRepository.Save();
                TempData["success"] = "Villa created successfully.";
                return RedirectToAction("Index", "Villa");
            }
            return View(villa);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var villa =villaRepository.Get(v => v.Id == id);
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
                villaRepository.Update(villa);
                villaRepository.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
