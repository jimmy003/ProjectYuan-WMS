using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Customer
{
    public class PriceListCustomer
    {
        public long PriceListId { get; set; }
        public List<long> CustomerIds { get; set; } = new List<long>();
    }
}
