using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        //private readonly IVillaRepository villaRepository;
        private readonly IUnitOfWork unitOfWork;
        public VillaController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;   
        }
        public IActionResult Index()
        {
            var villas = unitOfWork.Villa.GetAll();
            return View(villas);
        }

        public IActionResult Details(int id)
        {
            var villa = unitOfWork.Villa.Get(v => v.Id == id);
            return View(villa);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var villa = unitOfWork.Villa.Get(v => v.Id == id);
            unitOfWork.Villa.Remove(villa);
            if (villa is null)
            {
                return NotFound();

            }
             return View(villa);
        }
        [HttpPost]
        public IActionResult Delete(Villa villa)
        {   
            var villaFromDb = unitOfWork.Villa.Get(v => v.Id == villa.Id);
            if (villaFromDb is not null)
            {
                unitOfWork.Villa.Remove(villaFromDb);
                unitOfWork.Villa.Save();
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
                unitOfWork.Villa.Add(villa);
                unitOfWork.Villa.Save();
                TempData["success"] = "Villa created successfully.";
                return RedirectToAction("Index", "Villa");
            }
            return View(villa);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var villa = unitOfWork.Villa.Get(v => v.Id == id);
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
                unitOfWork.Villa.Update(villa);
                unitOfWork.Villa.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
