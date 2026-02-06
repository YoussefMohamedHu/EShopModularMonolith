using catalog.Products.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Description).HasMaxLength(200);
            builder.Property(p => p.ImageFile).HasMaxLength(200);
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.Category)
                   .HasConversion(
                       v => string.Join(',', v),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                   .HasMaxLength(500);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        }
    }
}
