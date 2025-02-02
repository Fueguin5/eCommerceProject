using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommercePlatform.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }

        public string? Display
        {
            get
            {
                return $"{Id}. {Name}\tPrice: ${Price}\tQuantity: {Quantity}";
            }
        }
        public Product()
        {
            Name = string.Empty;
            Price = -1;
            Quantity = -1;
        }

        public override string ToString()
        {
            return Display ?? string.Empty;
        }
    }
}
