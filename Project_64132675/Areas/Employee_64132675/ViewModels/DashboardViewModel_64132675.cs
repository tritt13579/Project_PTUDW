using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_64132675.Areas.Employee_64132675.ViewModels
{
    public class DashboardViewModel_64132675
    {
        public List<RevenueData> RevenueData { get; set; }
        public RoomStatusData RoomStatusData { get; set; }
        public List<ServiceUsageData> ServiceUsageData { get; set; }
    }

    public class RevenueData
    {
        public string Label { get; set; }
        public decimal Amount { get; set; }
    }

    public class RoomStatusData
    {
        public int OccupiedRooms { get; set; }
        public int BookedRooms { get; set; }
        public int AvailableRooms { get; set; }
    }

    public class ServiceUsageData
    {
        public string ServiceName { get; set; }
        public int UsageCount { get; set; }
    }
}