namespace Project_64132675.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROOMCLASS")]
    public partial class ROOMCLASS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ROOMCLASS()
        {
            ROOM = new HashSet<ROOM>();
            FEATURE = new HashSet<FEATURE>();
        }

        [Key]
        public long ROOM_CLASS_ID { get; set; }

        [Required]
        [StringLength(30)]
        public string CLASS_NAME { get; set; }

        public decimal BASE_PRICE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ROOM> ROOM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FEATURE> FEATURE { get; set; }
    }
}
