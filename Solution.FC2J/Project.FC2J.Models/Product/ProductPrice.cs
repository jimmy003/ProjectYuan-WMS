namespace Project.FC2J.Models.Product
{
    public class ProductPrice
    {
        public long Id { get; set; }
        public long PriceListId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal UnitDiscount { get; set; }
    }
}
