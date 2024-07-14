using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IVillaNumberRepository
    {
        IEnumerable<VillaNumber> GetAll(Expression<Func<VillaNumber , bool>>? filter=null,string? includeProperties=null );
        VillaNumber Get(Expression<Func<VillaNumber, bool>> filter , string? includeProperties = null);

        void Add(VillaNumber entity);
        void Update(VillaNumber entity);
        void Remove(VillaNumber entity);
        void Save();

    }
}
