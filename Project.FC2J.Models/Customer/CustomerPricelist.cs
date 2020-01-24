using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Customer
{
    public class CustomerPricelist
    {
        public Int64 Id { get; set; }
        public List<PriceList> PriceListDetails { get; set; } = new List<PriceList>();
    }
}
