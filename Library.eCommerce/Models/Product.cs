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
        public int? NumInStock { get; set; }

        public string? Display
        {
            get
            {
                return $"{Id}. {Name}\tPrice: ${Price}\tIn stock: {NumInStock}";
            }
        }
        public Product()
        {
            Name = string.Empty;
            Price = -1;
            NumInStock = -1;
        }

        public override string ToString()
        {
            return Display ?? string.Empty;
        }
    }
}
