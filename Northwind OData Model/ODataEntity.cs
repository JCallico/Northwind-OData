using System;
using System.Linq.Expressions;

namespace GSA.Samples.Northwind.OData.Model
{
    public abstract class ODataEntity<TEntity, TKey> : IODataEntity<TEntity, TKey>
    {
        public abstract Expression<Func<TEntity, bool>> HasID(TKey identifierToCompare);
    }
}