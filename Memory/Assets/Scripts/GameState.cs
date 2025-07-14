using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using static CreateCards;
using UnityEngine.Analytics;
using System.Collections;
using NUnit.Framework;
using System.Linq;

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
    public int stage = -1; // current stage, reference levelState
    public int numberOfTurns; // 0, 1 or 2

    [Header("Level & asset files")]
    [SerializeField] string levelDataFile = "levelData.json";
    [SerializeField] string cardJsonFile = "cardData.json";

    // We use cardobject so we can access the card data and the gameobject at the same time for bookkeeping (gameState) but also animation (gameObject)
    CardObject currentCardObject;
    CardObject previousCardObject;

    public List<Card> deck; // all cards from all levels, loaded once
    public List<CardObject> cards; // cards used for current level with gameobjects attached to each card
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


    public void CardClicked(int cardIndex)
    {
        if (cards[cardIndex].Data.CurrentState == Card.State.Finished)
        {
            return;
        }

        cards[cardIndex].Data.SetState(Card.State.FaceUp);
        
        // first card clicked
        if(currentCardObject == null)
        {
            currentCardObject = cards[cardIndex];
            //go.transform.GetChild(0).GetComponent<Interaction>()
            var currentCardInteraction = currentCardObject.View.transform.GetChild(0).GetComponent<Interaction>();
            //CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.DelayedFlip(1f, true));
            currentCardInteraction.FlipCard(true);


            currentCardObject.Data.SetState(Card.State.FaceUp);
            return;
        }
        // second card clicked
        if (previousCardObject == null)
        {
            previousCardObject = currentCardObject;
            currentCardObject = cards[cardIndex];
            currentCardObject.Data.SetState(Card.State.FaceUp);

            var currentCardInteraction = currentCardObject.View.transform.GetChild(0).GetComponent<Interaction>();
            var previousCardInteraction = previousCardObject.View.transform.GetChild(0).GetComponent<Interaction>();

            currentCardInteraction.FlipCard(true);


            if (currentCardObject.Data.GroupIndex == previousCardObject.Data.GroupIndex)
            {
                // do something, like remove the cards or mark them as matched
                Debug.Log("Cards match!");
                previousCardObject.Data.SetState(Card.State.Finished);
                currentCardObject.Data.SetState(Card.State.Finished);
                previousCardObject = null; // reset previous card
            }
            else
            {
                // flip these cards back down
                currentCardObject.Data.SetState(Card.State.FaceDown);
                previousCardObject.Data.SetState(Card.State.FaceDown);
                Debug.Log("Flipping cards back!");

                //CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.Delay(1));

                CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.DelayedFlipCard(false, 1));
                CoroutineRunner.Instance.RunCoroutine(previousCardInteraction.DelayedFlipCard(false, 1));
                //CoroutineRunner.Instance.RunCoroutine(previousCardInteraction.Delay(1));

                //currentCardInteraction.Flip(false, 1);
                //previousCardInteraction.Flip(false, 1);

                Debug.Log("Cards do not match!");

                previousCardObject = null;
                currentCardObject = null;
                return;
            }

          
        }

        //check how many cards are totally turned over


        previousCardObject = null;
        currentCardObject = null;
    }


    public bool isLevelDone()
    {
        return cards.All(item => item.Data.CurrentState==Card.State.Finished);
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
