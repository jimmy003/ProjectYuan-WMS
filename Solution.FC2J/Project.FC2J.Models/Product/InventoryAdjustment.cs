namespace Project.FC2J.Models.Product
{
    public class InventoryAdjustment
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public float OriginalQuantity { get; set; }
        public float Quantity { get; set; }
        public string Supplier { get; set; }
        public bool Action { get; set; }
        public string Remarks { get; set; }
        public string RequestBy { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
    }
}
