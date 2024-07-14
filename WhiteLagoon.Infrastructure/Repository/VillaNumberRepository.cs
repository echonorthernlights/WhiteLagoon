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
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext context;
        internal DbSet<VillaNumber> dbSet;
        public VillaNumberRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
            dbSet = this.context.Set<VillaNumber>();
        }

        //public void Add(VillaNumber entity)
        //{
        //    dbSet.Add(entity);
        //}

        //public VillaNumber Get(Expression<Func<VillaNumber, bool>> filter, string? includeProperties = null)
        //{
        //    IQueryable<VillaNumber> query = dbSet;
        //    if (filter is not null)
        //    {
        //        query = query.Where(filter);
        //    }
        //    if (!string.IsNullOrEmpty(includeProperties))
        //    {
        //        foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ){
        //            query = query.Include(property);
        //        }
        //    }
        //    return query.FirstOrDefault();
        //}

        //public IEnumerable<VillaNumber> GetAll(Expression<Func<VillaNumber, bool>>? filter = null, string? includeProperties = null)
        //{
        //    IQueryable<VillaNumber> query =dbSet;
        //    if (filter is not null) { 
        //        query = query.Where(filter);
        //    }
        //    if (!string.IsNullOrEmpty(includeProperties)) {
        //        foreach (var property in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)) {
        //            query = query.Include(property);
        //        }
        //    }
        //        return query.ToList();
        //    }  


        //public void Remove(VillaNumber entity)
        //{
        //    dbSet.Remove(entity);
        //}

        public void Save()
        {
           context.SaveChanges();
        }

        public void Update(VillaNumber entity)
        {
           context.Update(entity);
        }
    }
}
