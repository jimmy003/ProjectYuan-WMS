using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Sale
{
    public class ReceiveInvoice
    {
        public Invoice Invoice { get; set; } = new Invoice();
        public SaleHeader SaleHeader { get; set; } = new SaleHeader();
        public List<SaleDetail> Returns { get; set; } = new List<SaleDetail>();
    }
}
