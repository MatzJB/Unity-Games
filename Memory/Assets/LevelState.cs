using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LevelState
{
    //internal int cardsToMatch;

    [JsonProperty("stage")] public int Stage { get; set; }
    [JsonProperty("stageName")] public string StageName { get; set; }
    [JsonProperty("cardsToMatch")] public int CardsToMatch { get; set; }
    [JsonProperty("rows")] public int Rows { get; set; }
    [JsonProperty("category")] public string Category { get; set; }

    [JsonIgnore]
    public int Columns => CardsToMatch / Rows;
}
