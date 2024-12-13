namespace Project_64132675.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BEDTYPE")]
    public partial class BEDTYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BEDTYPE()
        {
            ROOMCLASSBEDTYPE = new HashSet<ROOMCLASSBEDTYPE>();
        }

        [Key]
        public long BED_TYPE_ID { get; set; }

        [Required]
        [StringLength(30)]
        public string BED_TYPE_NAME { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ROOMCLASSBEDTYPE> ROOMCLASSBEDTYPE { get; set; }
    }
}
