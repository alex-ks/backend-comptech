using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.Repositories
{
    public interface IRepository<TEntity> : IDisposable
    {
        bool Add(TEntity entity);
        bool Update(TEntity entity);
    }
}
