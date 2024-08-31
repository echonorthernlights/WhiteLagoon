using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
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
        [HttpGet]
        public IActionResult Index()
        {
            HomeViewModel model = new()
            {
                VillaList = unitOfWork.Villa.GetAll(includeProperties: "VillaAmenities"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),

            };
            return View(model);
        }

        //[HttpPost]
        //public IActionResult Index(HomeViewModel model)
        //{
        //    model.VillaList = unitOfWork.Villa.GetAll(includeProperties: "VillaAmenities");
        //    foreach (var villa in model.VillaList)
        //    {
        //        if (villa.Id % 2 == 0)
        //        {
        //            villa.IsAvailable = false;
        //        }
        //    }
        //    return View(model);
        //}

        [HttpPost]
        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {
            //Thread.Sleep(2000);
            var VillaList = unitOfWork.Villa.GetAll(includeProperties: "VillaAmenities").ToList();
            var VillaNumbersList = unitOfWork.VillaNumber.GetAll().ToList();
            var BookedVillas = unitOfWork.Booking.GetAll(b => b.Status == SD.StatusApproved || b.Status == SD.StatusCheckedIn).ToList();

            foreach (var villa in VillaList)
            {
                int roomAvailable = SD.VillaRoomsAvailable_Count(villa.Id, VillaNumbersList, checkInDate, nights, BookedVillas);
                villa.IsAvailable = roomAvailable > 0 ? true : false;
            }
            HomeViewModel homeViewModel = new()
            {
                CheckInDate = checkInDate,
                Nights = nights,
                VillaList = VillaList
            };
            return PartialView("_VillaList", homeViewModel);
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
