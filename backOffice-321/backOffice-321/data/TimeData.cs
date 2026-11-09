using System.Text.Json.Serialization;

namespace backOffice_321
{
    public class TimeData
    {
        [JsonPropertyName("UnixTimeStamp")]
        public int UnixTimeStamp { get; set; }
    }
}