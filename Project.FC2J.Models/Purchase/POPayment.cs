using System;
using System.Collections.Generic;

namespace Project.FC2J.Models.Purchase
{
    public class POPayment
    {
        public long Id { get; set; }
        public long OrderHeaderId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Amount { get; set; }
        public string UserName { get; set; }
        public bool IsCash { get; set; } 
        public string CashRemarks { get; set; }
        public string CheckBank { get; set; }
        public string CheckNumber { get; set; }
        public DateTime CheckDate { get; set; }
        public List<MyData> items { get; set; } = new List<MyData>();
    }
}
