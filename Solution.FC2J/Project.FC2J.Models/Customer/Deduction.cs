using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Customer
{
    public class Deduction
    {
        public long CustomerId { get; set; }
        public long Id { get; set; }
        public string Particular { get; set; }
        public decimal Amount { get; set; }
        public string PONo { get; set; }
        public decimal UsedAmount { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
