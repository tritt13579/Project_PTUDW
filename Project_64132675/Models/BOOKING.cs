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

        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public long CUSTOMER_ID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái thanh toán")]
        public long PAYMENT_STATUS_ID { get; set; }

        public DateTime BOOKING_DATE { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày nhận phòng")]
        public DateTime CHECKIN_DATE { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày trả phòng")]
        public DateTime CHECKOUT_DATE { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số người lớn")]
        [Range(1, int.MaxValue, ErrorMessage = "Số người lớn phải lớn hơn 0")]
        public byte NUM_ADULT { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số trẻ em")]
        [Range(0, int.MaxValue, ErrorMessage = "Số trẻ em không được âm")]
        public byte NUM_CHILDREN { get; set; }

        [StringLength(255)]
        public string SPECIAL_REQUESTS { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nguồn đặt phòng")]
        [StringLength(50)]
        public string BOOKING_SOURCE { get; set; }

        [Required(ErrorMessage = "Thành tiền không được để trống")]
        [Range(0, 99999999, ErrorMessage = "Thành tiền phải từ 0 đến 99,999,999")]
        public decimal BOOKING_AMOUNT { get; set; }

        public virtual PAYMENTSTATUS PAYMENTSTATUS { get; set; }

        public virtual CUSTOMER CUSTOMER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ROOM> ROOM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICE> SERVICE { get; set; }
    }
}
