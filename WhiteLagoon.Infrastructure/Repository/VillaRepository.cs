using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class VillaRepository : Repository<Villa>,  IVillaRepository
    {
        private readonly ApplicationDbContext context;
        public VillaRepository(ApplicationDbContext context) : base(context) 
        {
            this.context = context;
        }
        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Villa entity)
        {
            context.Update(entity);
        }
    }
}
