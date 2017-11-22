using System;

namespace GSA.Samples.Northwind.OData.Models
{
    public interface IODataEntity<TKey>
    {
        TKey ID { get; set; }
    }
}