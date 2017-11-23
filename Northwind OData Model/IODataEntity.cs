using System;
using System.Linq.Expressions;

namespace GSA.Samples.Northwind.OData.Model
{
    public interface IODataEntity<TEntity, in TKey>
    {
        Expression<Func<TEntity, bool>> HasID(TKey identifierToCompare);
    }
}