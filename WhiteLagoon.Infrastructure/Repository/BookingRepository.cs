using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext context;

        public BookingRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Booking entity)
        {
            context.Bookings.Update(entity);
        }
        public void UpdateStatus(int bookingId, string orderStatus, int villaNumber=0) {
        var bookingFromDb = context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (bookingFromDb != null) { 
                bookingFromDb.Status = orderStatus;
                if (bookingFromDb.Status == SD.StatusCheckedIn) {
                    bookingFromDb.VillaNumber = villaNumber;
                    bookingFromDb.ActualCheckInDate = DateTime.UtcNow;
                }
                if (bookingFromDb.Status == SD.StatusCompleted)
                {
                    bookingFromDb.ActualCheckOutDate = DateTime.UtcNow;
                }
                //context.SaveChanges();
            }
        }
        public void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId) {
            var bookingFromDb = context.Bookings.FirstOrDefault(b => b.Id == bookingId);

            if (bookingFromDb != null) {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    bookingFromDb.StripeSessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    bookingFromDb.StripePaymentIntentId = paymentIntentId;
                    bookingFromDb.PaymentDate = DateTime.UtcNow;
                    bookingFromDb.IsPaymentSuccessful = true;
                }
            
            }
           
        }


    }
}
