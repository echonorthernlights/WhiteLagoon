using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Web.Models;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWorkr)
        {
            this.unitOfWork = unitOfWorkr; 
        }

        public IActionResult Index()
        {
            HomeViewModel model = new(){
                VillaList = unitOfWork.Villa.GetAll(includeProperties: "VillaAmenities"),
                Nights=1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),

            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
