namespace Project_64132675.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PAYMENTSTATUS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PAYMENTSTATUS()
        {
            BOOKING = new HashSet<BOOKING>();
        }

        [Key]
        public long PAYMENT_STATUS_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string PAYMENT_STATUS_NAME { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BOOKING> BOOKING { get; set; }
    }
}
