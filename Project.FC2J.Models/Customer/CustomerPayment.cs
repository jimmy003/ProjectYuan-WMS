using System;
using System.Collections.Generic;

namespace Project.FC2J.Models.Customer
{
    public class CustomerPayment
    {
        public Int64 Id { get; set; }
        public List<Payment> PaymentDetails { get; set; } = new List<Payment>();
    }
}
