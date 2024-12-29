using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_64132675.Areas.Customer_64132675.Data
{
    public class RoomViewModel
    {
        public long RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string BedType { get; set; }
        public int NumBeds { get; set; }
        public string ClassName { get; set; }
        public decimal BasePrice { get; set; }
        public List<string> Features { get; set; }
    }
}