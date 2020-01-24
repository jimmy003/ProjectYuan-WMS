using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Product
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int CartQuantity { get; set; }
    }
}
