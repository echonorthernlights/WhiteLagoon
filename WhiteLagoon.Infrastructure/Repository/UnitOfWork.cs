using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
       
        public IVillaRepository Villa { get; private set; }

        public IVillaNumberRepository VillaNumber { get; private set; }

        public IAmenityRepository Amenity { get; private set; }

        public IBookingRepository Booking { get; private set; }

        public IApplicationUserRepository User { get; private set; } 
        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            Villa = new VillaRepository(context);
            VillaNumber = new VillaNumberRepository(context);
            Amenity = new AmenityRepository(context);
            Booking = new BookingRepository(context);
            User = new ApplicationUserRepository(context);
        }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}
