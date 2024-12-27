using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_64132675.Areas.Customer_64132675.Data
{
    public class BookingSummaryViewModel
    {
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public long? RoomClassId { get; set; }
    }
}