using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Customer
{
    public class CustomerProduct
    {
        public Int64 Id { get; set; }
        public List<Product.Product> ProductDetails { get; set; } = new List<Product.Product>();

    }
}
