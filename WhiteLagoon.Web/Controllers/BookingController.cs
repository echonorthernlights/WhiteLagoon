using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
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

        public IActionResult Index()
        {
            return View();
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

            //stripe payments
            var domain = Request.Scheme+"://"+Request.Host.Value+"/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"Booking/FinalizeBooking?nights={booking.Nights}&villaId={booking.VillaId}&checkInDate={booking.CheckInDate}",
              
            };

            options.LineItems.Add(new SessionLineItemOptions
            {

                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalCost * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                        //Images = new List<string> { domain + villa.ImageUrl },

                    },
                },
                Quantity = 1,
            });
            //create stripe payment session

            var service = new SessionService();
            Session session = service.Create(options);

            unitOfWork.Booking.UpdateStripePaymentId(booking.Id,session.Id,session.PaymentIntentId);
            unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


            //return RedirectToAction(nameof(BookingConfirmation), new {bookingId = booking.Id});
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            var booking = unitOfWork.Booking.Get(b=> b.Id == bookingId, includeProperties:"User,Villa");
            if(booking.Status == SD.StatusPending)
            {
                var service = new SessionService();
                var session = service.Get(booking.StripeSessionId);

                if (session.PaymentStatus == "paid") {
                    unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusApproved, 0);
                    unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
                    unitOfWork.Save();
                }
            }
            return View(bookingId);
        }
        [HttpGet]
        [Authorize]
        public IActionResult BookingDetails(int bookingId) {

            var bookingDetails = unitOfWork.Booking.Get(b => b.Id == bookingId, includeProperties: "User,Villa");

            if(bookingDetails.VillaNumber == 0 && bookingDetails.Status == SD.StatusApproved)
            {
                var availableVillaNumber = AssignAvailableVillaNumberByVilla(bookingDetails.VillaId);
                bookingDetails.VillaNumbers = unitOfWork.VillaNumber.GetAll(u => u.VillaId == bookingDetails.VillaId && availableVillaNumber.Any(x => x == u.Villa_Number)).ToList();
            }

            return View(bookingDetails);
        }
        #region API Calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;
            if (User.IsInRole(SD.Admin))
            {
                objBookings = unitOfWork.Booking.GetAll( includeProperties:"User,Villa");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity) User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objBookings = unitOfWork.Booking.GetAll(u=>u.UserId == userId ,includeProperties: "User,Villa");

            }
            if (!string.IsNullOrEmpty(status))
            {
                objBookings = objBookings.Where(x => x.Status.ToLower() == status.ToLower());
            }
            return Json(new { data = objBookings });
           
        }
        private List<int> AssignAvailableVillaNumberByVilla(int villaId) {
            List<int> availableVillaNumbers = new();
            var villaNumbers = unitOfWork.VillaNumber.GetAll(v=>v.VillaId == villaId);
            var checkedInVillas = unitOfWork.Booking.GetAll(b => b.VillaId == villaId && b.Status == SD.StatusCheckedIn)
                .Select(b=>b.VillaNumber);

            foreach (var villaNumber in villaNumbers) {
                if (!checkedInVillas.Contains(villaNumber.Villa_Number)) { 
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }
            return availableVillaNumbers;

        }
        #endregion
    }
}
