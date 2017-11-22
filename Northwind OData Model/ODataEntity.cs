using System;
using GSA.Samples.Northwind.OData.Models;

namespace GSA.Samples.Northwind.OData.Model
{
    public abstract class ODataEntity<TKey> : IODataEntity<TKey>
    {
        public abstract TKey ID { get; set; }
    }
}