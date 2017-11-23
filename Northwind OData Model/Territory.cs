using System.Linq.Expressions;

namespace GSA.Samples.Northwind.OData.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Territory : ODataEntity<Territory, string>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Territory()
        {
            Employees = new HashSet<Employee>();
        }

        [StringLength(20)]
        public string TerritoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string TerritoryDescription { get; set; }

        public int RegionID { get; set; }

        public virtual Region Region { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employees { get; set; }

        #region IODataEntity members implementation

        public override Expression<Func<Territory, bool>> HasID(string identifierToCompare)
        {
            return e => e.TerritoryID == identifierToCompare;
        }

        #endregion
    }
}
