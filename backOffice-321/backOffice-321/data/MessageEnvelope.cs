using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace backOffice_321.data
{
    public class MessageEnvelope
    {
        [JsonPropertyName("v")]
        public int V { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("ts")]
        public string Ts { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("payload")]
        public JsonObject Payload { get; set; }
    }
}