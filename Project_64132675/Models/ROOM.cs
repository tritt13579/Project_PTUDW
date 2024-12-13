namespace Project_64132675.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROOM")]
    public partial class ROOM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ROOM()
        {
            BOOKING = new HashSet<BOOKING>();
        }

        [Key]
        public long ROOM_ID { get; set; }

        public long FLOOR_ID { get; set; }

        public long ROOM_CLASS_ID { get; set; }

        public long ROOM_STATUS_ID { get; set; }

        public int ROOM_NUMBER { get; set; }

        public virtual FLOOR FLOOR { get; set; }

        public virtual ROOMCLASS ROOMCLASS { get; set; }

        public virtual ROOMSTATUS ROOMSTATUS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BOOKING> BOOKING { get; set; }
    }
}
