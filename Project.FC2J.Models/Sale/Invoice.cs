using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Sale
{
    public class Invoice
    {
        public long Id { get; set; }
        public string PONo { get; set; }
        public long CustomerId { get; set; }
        public bool IsReceived { get; set; }
        public bool WithReturns { get; set; }
    }
}
