using Newtonsoft.Json;

public class CardAsset
{
    [JsonIgnore]
    public int State { get; set; }

    [JsonProperty("groupIndex")]
    public int GroupIndex { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }
}
