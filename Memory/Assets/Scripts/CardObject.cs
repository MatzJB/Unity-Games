using System;
using UnityEngine;

public class CardObject
{
    public Card Data { get; set; }
    public GameObject View { get; set; }

    public CardObject(Card data, GameObject view)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        View = view ?? throw new ArgumentNullException(nameof(view));
    }
}

