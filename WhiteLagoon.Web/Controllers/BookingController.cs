using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult FinalizeBooking(int nights, DateOnly checkInDate, int villaId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = unitOfWork.User.Get(u => u.Id == userId);

            Booking booking = new()
            {
                VillaId = villaId,
                Villa = unitOfWork.Villa.Get(v => v.Id == villaId, includeProperties: "VillaAmenities"),
                Nights = nights,
                CheckInDate = checkInDate,
                CheckOutDate = checkInDate.AddDays(nights),
                Name = user.Name,
                Phone=user.PhoneNumber,
                Email = user.Email,
                UserId = user.Id
               

            };
            booking.TotalCost = nights * booking.Villa.Price;
            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        { 
            var villa = unitOfWork.Villa.Get(v=> v.Id == booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.UtcNow;

            unitOfWork.Booking.Add(booking);
            unitOfWork.Save();

            return RedirectToAction(nameof(BookingConfirmation), new {bookingId = booking.Id});
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            return View(bookingId);
        }
    }
}
