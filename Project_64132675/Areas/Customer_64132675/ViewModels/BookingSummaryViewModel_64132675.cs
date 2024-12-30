using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_64132675.Areas.Customer_64132675.ViewModels
{
    public class BookingSummaryViewModel_64132675
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public byte Adults { get; set; } = 2;
        public byte Children { get; set; } = 0;
        public int Nights
        {
            get
            {
                return (CheckOut.Date - CheckIn.Date).Days;
            }
        }
        public string SpecialRequests { get; set; }
        public List<RoomSelection> SelectedRooms { get; set; }
        public List<long> SelectedServices { get; set; }
        public List<ServiceDetail> SelectedServiceDetails { get; set; }
        public decimal TotalRoomAmount { get; set; }
        public decimal TotalServiceAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal TotalAmountUSD => Math.Round(TotalAmount / 24240m, 2);

        public BookingSummaryViewModel_64132675()
        {
            CheckIn = DateTime.Now;
            CheckOut = DateTime.Now.AddDays(1);
            Adults = 2;
            Children = 0;
            SelectedRooms = new List<RoomSelection>();
            SelectedServices = new List<long>();
            SelectedServiceDetails = new List<ServiceDetail>();
        }
    }

    public class RoomSelection
    {
        public string RoomClassId { get; set; }
        public string RoomNumber { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
    }
    public class ServiceDetail
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
    }
}