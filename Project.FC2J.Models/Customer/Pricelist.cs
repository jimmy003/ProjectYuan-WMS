namespace Project.FC2J.Models.Customer
{
    public class PriceList
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool DiscountPolicy { get; set; }
        public int Subscribed { get; set; }
        public bool Deleted { get; set; }
        public bool IsForSalesOrder { get; set; }
        public string Email { get; set; }
    }

}
