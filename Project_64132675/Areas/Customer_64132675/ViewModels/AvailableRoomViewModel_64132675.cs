using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_64132675.Areas.Customer_64132675.ViewModels
{
    public class AvailableRoomViewModel_64132675
    {
        public long RoomClassId { get; set; }
        public string ClassName { get; set; }
        public decimal BasePrice { get; set; }
        public string BedType { get; set; }
        public int NumBeds { get; set; }
        public int AvailableCount { get; set; }
        public string RoomNumbers { get; set; }
        public List<string> Features { get; set; }
    }
}