using System.Text.Json.Serialization;

namespace backOffice_321
{
    public class TimeData
    {
        [JsonPropertyName("UnixTimeStamp")]
        public long UnixTimeStamp { get; set; }
    }
}