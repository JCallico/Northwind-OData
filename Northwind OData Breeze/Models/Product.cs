using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSA.Samples.Northwind.OData.Models
{
    [Table("Products")]
    //[Page(MaxTop = 1000, PageSize = 50)]
    //[Count(Disabled = false)]
    public class Product : BaseEntity, IEntity
    {
        [Key]
        [Column("ProductUniqueID")]
        public override Guid ID { get; set; }

        [Required]
        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }


        public bool Discontinued { get; set; }
    }
}