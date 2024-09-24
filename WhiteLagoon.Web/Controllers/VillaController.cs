using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        //private readonly IVillaRepository villaRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
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
                if (!string.IsNullOrEmpty(villaFromDb.ImageUrl))
                {
                    string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, villaFromDb.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
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
        public IActionResult Create(Villa villa)
        {

            if (villa.Name == villa.Description)
            {
                ModelState.AddModelError("Name", "The name and description cannot match");
            }
            if (ModelState.IsValid)
            {
                if (villa.Image is not null)
                {
                    //string imagePath = Path.Combine(webHostEnvironment.WebRootPath, @"images\Villa");
                    // Using Path.Combine for cross-platform compatibility
                    string imagePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "Villa");

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);

                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName).Replace("\\", "/"), FileMode.Create);
                    villa.Image.CopyTo(fileStream);

                    //villa.ImageUrl = @"\images\Villa\" + fileName;
                    villa.ImageUrl = $"/images/Villa/{fileName}";
                    //villa.ImageUrl = Path.Combine("images", "Villa", fileName).Replace("\\", "/");

                }
                else
                {
                    villa.ImageUrl = "https://via.assets.so/img.png?w=600&h=400&tc=blue&bg=white";
                }
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
                if (villa.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                    //string imagePath = Path.Combine(webHostEnvironment.WebRootPath, @"images\Villa");
                    string imagePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "Villa");
                    if (!string.IsNullOrEmpty(villa.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName).Replace("\\", "/"), FileMode.Create);
                    villa.Image.CopyTo(fileStream);

                    //villa.ImageUrl = @"\images\Villa\" + fileName;
                    villa.ImageUrl = $"/images/Villa/{fileName}";
                }
                unitOfWork.Villa.Update(villa);
                unitOfWork.Villa.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
