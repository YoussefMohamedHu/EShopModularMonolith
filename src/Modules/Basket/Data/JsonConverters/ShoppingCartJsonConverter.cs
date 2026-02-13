using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using basket.Basket.Models;

namespace basket.Data.JsonConverters;

internal sealed class ShoppingCartJsonConverter : JsonConverter<ShoppingCart>
{
    private static readonly FieldInfo ItemsField = typeof(ShoppingCart).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic)!;

    public override ShoppingCart Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var data = JsonSerializer.Deserialize<ShoppingCartData>(ref reader, options)
            ?? throw new JsonException("Invalid ShoppingCart payload.");

        var cart = ShoppingCart.CreateShoppingCart(data.Id, data.UserName ?? string.Empty);

        var list = (List<ShoppingCartItem>)ItemsField.GetValue(cart)!;
        if (data.Items != null)
        {
            list.AddRange(data.Items);
        }

        return cart;
    }

    public override void Write(Utf8JsonWriter writer, ShoppingCart value, JsonSerializerOptions options)
    {
        var data = new ShoppingCartData
        {
            Id = value.Id,
            UserName = value.UserName,
            Items = new List<ShoppingCartItem>(value.Items)
        };

        JsonSerializer.Serialize(writer, data, options);
    }

    private sealed class ShoppingCartData
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public List<ShoppingCartItem>? Items { get; set; }
    }
}
