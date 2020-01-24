using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Purchase
{
    public class PoDetail
    {
        public long Id { get; set; }
        public long PoHeaderId { get; set; }
        public int LineNo { get; set; }
        public long ProductId { get; set; }
        public double Quantity { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal SalePrice { get; set; }
        public decimal UnitDiscount { get; set; }
        public string UnitOfMeasure { get; set; }
        public string SFAUnitOfMeasure { get; set; }
        public string SFAReferenceNo { get; set; }
        public string NetWeight { get; set; }
        public string TaxType { get; set; }

        public decimal SubTotal { get; set; }
        public bool IsTaxable { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxPrice { get; set; }
        public bool IsDelivered { get; set; }
        public string InvoiceNo { get; set; }

    }
}
