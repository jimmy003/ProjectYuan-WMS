using System;

namespace Project.FC2J.Models.Report
{
    public class DailyInventory
    {
        public long Id { get; set; }
        public DateTime inventoryDate { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Sinulid { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal Quantity { get; set; }
    }
}
