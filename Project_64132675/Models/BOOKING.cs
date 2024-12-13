namespace Project_64132675.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BOOKING")]
    public partial class BOOKING
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BOOKING()
        {
            ROOM = new HashSet<ROOM>();
            SERVICE = new HashSet<SERVICE>();
        }

        [Key]
        public long BOOKING_ID { get; set; }

        public long CUSTOMER_ID { get; set; }

        public long PAYMENT_STATUS_ID { get; set; }

        public DateTime BOOKING_DATE { get; set; }

        public DateTime CHECKIN_DATE { get; set; }

        public DateTime CHECKOUT_DATE { get; set; }

        public byte NUM_ADULT { get; set; }

        public byte NUM_CHILDREN { get; set; }

        [StringLength(255)]
        public string SPECIAL_REQUESTS { get; set; }

        [Required]
        [StringLength(50)]
        public string BOOKING_SOURCE { get; set; }

        public decimal BOOKING_AMOUNT { get; set; }

        public virtual PAYMENTSTATUS PAYMENTSTATUS { get; set; }

        public virtual CUSTOMER CUSTOMER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ROOM> ROOM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICE> SERVICE { get; set; }
    }
}
