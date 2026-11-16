using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backOffice_321.data
{
    public class Product
    {
        [JsonPropertyName("ID")]
        [RegularExpression(@"^PROD-[0-9]{4}$")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Picture")]
        public string Picture { get; set; } = string.Empty; // base64

        [JsonPropertyName("Price")]
        public decimal Price { get; set; }
    }

    public class BusinessProfile
    {
        [JsonPropertyName("ID")]
        [RegularExpression(@"^COM-[0-9]{4}$")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Logo")]
        public string Logo { get; set; } = string.Empty; // base64

        [JsonPropertyName("Special")]
        public Product Special { get; set; } = new Product();

        public List<Product> Products { get; set; } = new List<Product>();
    }
}