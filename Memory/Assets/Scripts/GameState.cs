using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using static CreateCards;
using UnityEngine.Analytics;


/*
using System.Collections.Generic;

[System.Serializable]           // lets Unity serialize it if you expose it
public class GameState
{
    public List<LevelState> levels = new();
    public int stage = -1;
    public int numberOfTurns = 0;

    public string levelDataFile = "levelData.json";
    public string cardJsonFile = "cardData.json";

    public Card currentCard;
    public Card previousCard;

    public List<CardObject> cards = new();
    public List<Card> deck = new();
    public List<LevelState> levelStates = new();

    public GameState() { }             // init already done in field initialisers
}
*/



/* This class contains the game state of the game, level data, bonuses and penalties et cetera */
// add state for menu, pause, running and end, replay
public class GameState
{
    public int stage = -1; // current stage
    public int numberOfTurns; // 0, 1 or 2

    [Header("Level & asset files")]
    [SerializeField] string levelDataFile = "levelData.json";
    [SerializeField] string cardJsonFile = "cardData.json";

    Card currentCard;
    Card previousCard;

    public List<CardObject> cards; // cards used for current level with gameobjects attached to each card
    public List<Card> deck; // all cards from all levels, loaded once
    public List<LevelState> levelStates;
    //TODO: this is a gameObject property, can I add this to the card list as a property?


    public GameState()
    {
        // loading level data
        stage = 0;

        //TODO: add a high scores JSON?
        string cardDataFilename = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, cardJsonFile));
        string levelDataFilename = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, levelDataFile));

        deck = JsonConvert.DeserializeObject<List<Card>>(cardDataFilename);
        levelStates = JsonConvert.DeserializeObject<List<LevelState>>(levelDataFilename);
        //TODO: add a way to get the cards for this level based on deck and randomizer
        // create a new card for this level

         //Debug.Log($"Loaded {cards.Count} cards for stage {stage}.");
        //StartCoroutine(FaceUpAllCards(true, 0, true));
        //StartCoroutine(FaceUpAllCards(false, 2, true));
    }


    // should be here?
    public void CardClicked(Interaction card)
    {
        if (currentCard == null)
        {
            currentCard = card.GetComponent<Card>();
            currentCard.SetState(Card.State.FaceUp);
        }
        else if (previousCard == null)
        {
            previousCard = currentCard;
            currentCard = card.GetComponent<Card>();
            currentCard.SetState(Card.State.FaceUp);
            // check if the two cards match
            if (currentCard.GroupIndex == previousCard.GroupIndex)
            {
                // do something, like remove the cards or mark them as matched
                Debug.Log("Cards match!");
                previousCard.SetState(Card.State.Finished);
                currentCard.SetState(Card.State.Finished);
                previousCard = null; // reset previous card
            }
            else
            {
                // do something, like flip them back after a delay

                Debug.Log("Cards do not match!");
                //TODO: fix this, needs to flip up
                //StartCoroutine(FaceUpAllCards(false, 2, true));



                previousCard = null; // reset previous card
            }
        }

    }

    public void FlipAll()
    {
        foreach (var card in cards) card.Data.SetState(Card.State.FaceUp);
    }



    void RandomizeDeck()
    {
        Assets.Misc.Randomize(cards);

    }


    // Rotates the cards around the center of the cloud
    void Tornado()
    {
    }

    // Reveals the cards by lighting a light bulb
    void Idea()
    {
    }

    // Spins all cards a couple turns
    // requires the cards the themselves have knowledge about their position
    void Wind()
    {
    }

    void NextLevel()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
