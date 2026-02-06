using catalog.Products.Events;
using shared.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Models
{
    public class Product : Aggregate<Guid>
    {
        public string Name { get; set; } = default!;
        public List<string> Category { get; set; } = new();
        public string Description { get; set; } = default!;
        public string ImageFile { get; set; } = default!;
        public decimal Price { get; set; }

        public static Product Create(Guid id,string name, List<string> category,string description,string imageFile, decimal price)
        {
            var product =  new Product
            {
                Id = id,
                Name = name,
                Category = category,
                Description = description,
                ImageFile = imageFile,
                Price = price
            };
            product.AddDomainEvent(new ProductCreatedEvent(product));
            return product;
        }
        public void Update(string name, List<string> category, string description, string imageFile, decimal price)
        {
            Name = name;
            Category = category;
            Description = description;
            ImageFile = imageFile;
            
            
            if(Price != price)
            {
                Price = price;
                AddDomainEvent(new ProductPriceChangedEvent(this));
            }
        }
    }
}
