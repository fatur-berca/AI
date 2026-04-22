using DFIS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TOM.EntitiesDAL
{
    public interface ITOMGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity:class
    {
    }
}
