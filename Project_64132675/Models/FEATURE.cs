namespace Project_64132675.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FEATURE")]
    public partial class FEATURE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FEATURE()
        {
            ROOMCLASS = new HashSet<ROOMCLASS>();
        }

        [Key]
        public long FEATURE_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string FEATURE_NAME { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ROOMCLASS> ROOMCLASS { get; set; }
    }
}
