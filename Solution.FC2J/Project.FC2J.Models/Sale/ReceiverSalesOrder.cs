using System;

namespace Project.FC2J.Models.Sale
{
    public class ReceiverSalesOrder
    {
        public string Date { get; set; }
        public string Partner { get; set; }
        public string SoNo { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}
