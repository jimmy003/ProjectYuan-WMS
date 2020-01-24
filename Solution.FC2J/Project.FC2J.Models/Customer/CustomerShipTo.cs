using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Customer
{
    public class CustomerShipTo
    {
        public Int64 Id { get; set; }
        public List<ShippingAddress> ShipToDetails { get; set; } = new List<ShippingAddress>();

    }
}
