using System.Text.Json;
using System.Text.Json.Serialization;
using basket.Basket.Models;

namespace basket.Data.JsonConverters;

internal sealed class ShoppingCartItemJsonConverter : JsonConverter<ShoppingCartItem>
{
    public override ShoppingCartItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var data = JsonSerializer.Deserialize<ShoppingCartItemData>(ref reader, options)
            ?? throw new JsonException("Invalid ShoppingCartItem payload.");

        var item = new ShoppingCartItem(
            data.ShoppingCartId,
            data.ProductId,
            data.Quantity,
            data.Color ?? string.Empty,
            data.Price,
            data.ProductName ?? string.Empty);

        item.Id = data.Id;
        return item;
    }

    public override void Write(Utf8JsonWriter writer, ShoppingCartItem value, JsonSerializerOptions options)
    {
        var data = new ShoppingCartItemData
        {
            Id = value.Id,
            ShoppingCartId = value.ShoppingCartId,
            ProductId = value.ProductId,
            Quantity = value.Quantity,
            Color = value.Color,
            Price = value.Price,
            ProductName = value.ProductName
        };

        JsonSerializer.Serialize(writer, data, options);
    }

    private sealed class ShoppingCartItemData
    {
        public Guid Id { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public string? ProductName { get; set; }
    }
}
