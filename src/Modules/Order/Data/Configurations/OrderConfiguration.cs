using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using order.Orders.Models;
using order.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace order.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            
            builder.ComplexProperty(o => o.ShippingAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.EmailAddress).HasColumnName("ShippingEmailAddress");
                addressBuilder.Property(a => a.AddressLine).HasColumnName("ShippingAddressLine");
                addressBuilder.Property(a => a.City).HasColumnName("ShippingCity");
                addressBuilder.Property(a => a.Country).HasColumnName("ShippingCountry");
            });
            
            builder.ComplexProperty(o => o.Payment, paymentBuilder =>
            {
                paymentBuilder.Property(p => p.CardName).HasColumnName("PaymentCardName");
                paymentBuilder.Property(p => p.CardNumber).HasColumnName("PaymentCardNumber");
                paymentBuilder.Property(p => p.ExpirationDate).HasColumnName("PaymentExpirationDate");
                paymentBuilder.Property(p => p.CVV).HasColumnName("PaymentCVV");
            });
        }
    }
}
