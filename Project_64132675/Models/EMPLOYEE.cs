namespace Project_64132675.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EMPLOYEE")]
    public partial class EMPLOYEE
    {
        [Key]
        public long EMPLOYEE_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string FIRST_NAME { get; set; }

        [Required]
        [StringLength(50)]
        public string LAST_NAME { get; set; }

        [Required]
        [StringLength(1)]
        public string GENDER { get; set; }

        public DateTime DATE_OF_BIRTH { get; set; }

        [Required]
        [StringLength(100)]
        public string EMAIL { get; set; }

        [Required]
        [StringLength(15)]
        public string PHONE_NUMBER { get; set; }

        public DateTime HIRE_DATE { get; set; }

        [Required]
        [StringLength(30)]
        public string ROLE_NAME { get; set; }

        [Required]
        [StringLength(255)]
        public string PASSWORD { get; set; }
    }
}
