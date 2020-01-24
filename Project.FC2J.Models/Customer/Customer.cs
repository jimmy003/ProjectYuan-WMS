using System;
using System.Collections.Generic;

namespace Project.FC2J.Models.Customer
{
    public class Customer
    {
        public Int64 Id { get; set; }
        public string ReferenceNo { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string MobileNo { get; set; }
        public string TelNo { get; set; }
        public string TIN { get; set; }
        public Int64 PaymentTypeId { get; set; }
        public string PaymentType { get; set; }
        public string PriceList { get; set; }
        public long PriceListId { get; set; }
        public bool Deleted { get; set; }
        public List<Payment> PaymentDetails { get; set; } = new List<Payment>();
        public List<PriceList> PriceListDetails { get; set; } = new List<PriceList>();
        public List<Product.Product> ProductDetails { get; set; } = new List<Product.Product>();
        public List<ShippingAddress> ShipTo { get; set; } = new List<ShippingAddress>();
        public long FarmId { get; set; }

    }
}