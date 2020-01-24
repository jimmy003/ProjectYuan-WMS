using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Customer
{
    public class Payment
    {
        public Int64 Id { get; set; }
        public string PaymentType { get; set; }
        public string Remarks { get; set; }
        public bool Deleted { get; set; }
    }
}
