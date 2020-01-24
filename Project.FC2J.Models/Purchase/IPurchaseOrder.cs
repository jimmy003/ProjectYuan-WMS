using System.Collections.Generic;

namespace Project.FC2J.Models.Purchase
{
    public interface IPurchaseOrder
    {
        PoHeader PoHeader { get; set; }
        IEnumerable<PoDetail> PoDetails { get; set; }
    }
}