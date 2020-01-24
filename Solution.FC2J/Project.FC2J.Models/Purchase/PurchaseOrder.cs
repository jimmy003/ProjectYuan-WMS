using System.Collections.Generic;

namespace Project.FC2J.Models.Purchase
{
    public class PurchaseOrder : IPurchaseOrder
    {
        public PoHeader PoHeader { get; set; } = new PoHeader();
        public bool IsWithReturns { get; set; }
        public IEnumerable<PoDetail> PoDetails { get; set; } = new List<PoDetail>();

    }
}
