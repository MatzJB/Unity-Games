using System;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Runtime wrapper around the data loaded from cards.json.
/// Keeps the *serialised* fields from CardAsset and adds a *non-serialised* State.
/// </summary>
public class Card
{
    
    public enum State : ushort
    {
        FaceUp = 0,
        FaceDown = 1,
        Finished = 2,
        Uninitialized = 3,
        //Frozen = 4
    }

    [JsonIgnore]
    public State CurrentState { get; private set; } = State.Uninitialized;

    [JsonProperty("groupIndex")]
    public int GroupIndex { get; private set; }

    [JsonProperty("id")]
    public int Id { get; private set; }

    [JsonProperty("category")]
    public string Category { get; private set; }

    [JsonProperty("path")]
    public string Path { get; private set; }

    public Card() { }

    public Card(CardAsset asset)
    {
        if (asset == null) throw new ArgumentNullException(nameof(asset));

        GroupIndex = asset.GroupIndex;
        Id = asset.Id;
        Category = asset.Category;
        Path = asset.Path;
        CurrentState = State.Uninitialized;
    }

    public Card(int groupIndex, int id, string category, string path)
    {
        GroupIndex = groupIndex;
        Id = id;
        Category = category;
        Path = path;
        CurrentState = State.Uninitialized;
    }

    public void SetState(State s) => CurrentState = s;

    public bool Matches(Card other) => other != null && GroupIndex == other.GroupIndex;

    public override string ToString() =>
        $"Card #{Id} [{Category}] ({CurrentState}) path=\"{Path}\"";
}