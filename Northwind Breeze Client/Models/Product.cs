using Breeze.Sharp;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSA.Samples.Northwind.OData.Models
{
    public class Product : BaseEntity
    {
        public Guid ID { get { return GetValue<Guid>(); } set { SetValue(value); } }

        public string ProductName { get { return GetValue<string>(); } set { SetValue(value); } }

        public decimal UnitPrice { get { return GetValue<decimal>(); } set { SetValue(value); } }

        public bool Discontinued { get { return GetValue<bool>(); } set { SetValue(value); } }

    }
}