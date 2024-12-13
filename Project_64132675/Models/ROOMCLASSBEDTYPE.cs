namespace Project_64132675.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROOMCLASSBEDTYPE")]
    public partial class ROOMCLASSBEDTYPE
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ROOM_CLASS_ID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long BED_TYPE_ID { get; set; }

        public byte NUM_BEDS { get; set; }

        public virtual BEDTYPE BEDTYPE { get; set; }

        public virtual ROOMCLASS ROOMCLASS { get; set; }
    }
}
