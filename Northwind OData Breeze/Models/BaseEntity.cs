using System;

namespace GSA.Samples.Northwind.OData.Models
{
    public abstract class BaseEntity : IEntity
    {
        public abstract Guid ID { get; set; }
    }
}