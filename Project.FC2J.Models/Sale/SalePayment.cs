using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Sale
{
    public class SalePayment
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string UserName { get; set; }
        public long OrderStatusId { get; set; }
        public string PONo { get; set; }
        public decimal PaidAmount { get; set; }
        public bool IsCash { get; set; }
        public string CashRemarks { get; set; }
        public string CheckBank { get; set; }
        public string CheckNumber { get; set; }
        public DateTime CheckDate { get; set; }
    }
}
