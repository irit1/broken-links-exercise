using Newtonsoft.Json;


namespace trendeamon.Models
{
    public class BrokenLinksStartRequestModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
