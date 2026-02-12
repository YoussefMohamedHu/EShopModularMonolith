using basket.Basket.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shared.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace basket.Data.Configurations
{
    public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            
                builder.HasKey(e => e.Id);
                builder.Property(e => e.ProductId).IsRequired();
                builder.Property(e => e.Quantity).IsRequired();
                builder.Property(e => e.Color).IsRequired().HasMaxLength(50);
                builder.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
           
        }
    }
}
