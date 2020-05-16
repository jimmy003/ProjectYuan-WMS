namespace Project.FC2J.Models.Sale
{
    public class Invoice
    {
        public long Id { get; set; }
        public string PoNo { get; set; }
        public long CustomerId { get; set; }
        public bool IsReceived { get; set; }
        public bool WithReturns { get; set; }
    }
}
