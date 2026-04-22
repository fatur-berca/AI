using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFIS.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetGenericRepository<T>()
          where T : class;

        /// <summary>
        /// Saves current context changes.
        /// </summary>
        void SaveChanges();

        void RevertChanges();

        //ISqlSPRepository GetSPRepository();
    }
}
