using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Customer
{
    public class PriceListProduct
    {
        public long PriceListId { get; set; }
        public List<ProductDeduction> ProductDeductions { get; set; } = new List<ProductDeduction>();

    }
}
