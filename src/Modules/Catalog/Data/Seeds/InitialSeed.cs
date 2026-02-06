using catalog.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Data.Seeds
{
    public static class InitialSeed
    {
        public static List<Product> Proudct()
        {
            return new List<Product>
            {
                new Product
                {
                    Name = "Sample Product 1",
                    Description = "This is a sample product description.",
                    Price = 19.99M,
                    ImageFile = "sample1.jpg",
                    Category = new List<string> { "Category1", "Category2" }

                },

                new Product
                {
                    Name = "Sample Product 2",
                    Description = "This is another sample product description.",
                    Price = 29.99M,
                    ImageFile = "sample2.jpg",
                    Category = new List<string> { "Category3", "Category4" }
                }
            };

        }
    }
}
