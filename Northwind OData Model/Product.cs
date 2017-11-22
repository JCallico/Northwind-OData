using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.OData.Query;

namespace GSA.Samples.Northwind.OData.Model
{
    [Table("Products")]
    [Page(MaxTop = 1000, PageSize = 50)]
    [Count(Disabled = false)]
    public partial class Product: ODataEntity<Guid>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            OrderDetails = new HashSet<Order_Detail>();
        }

        // Required by OData base entity
        [Column("ProductUniqueID")]
        public override Guid ID { get; set; }

        // Do not use Key attribute for entities that have an internal key for EF
        // and a different key property for OData
        // Define the EF key using fluent method (see NorthwindContext.OnModelCreating()
        // Define the OData key on ODataConventionModelBuilder (see NorthwindContext.GetConventionModel()
        // [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }

        public int? SupplierID { get; set; }

        public int? CategoryID { get; set; }

        [StringLength(20)]
        public string QuantityPerUnit { get; set; }

        [Column(TypeName = "money")]
        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public Guid ReferenceUniqueID { get; set; }

        [StringLength(500)]
        public string ProductUri { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Detail> OrderDetails { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}
