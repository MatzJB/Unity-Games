using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using static CreateCards;


/* This class contains the game state of the game, level data, bonuses and penalties et cetera */
// add state for menu, pause, running and end, replay
public class GameState : MonoBehaviour
{
    public List<LevelState> levels; //contains stage and all data for the current level
    public int stage = -1; // current stage
    public int numberOfTurns; // 0, 1 or 2

    [Header("Level & asset files")]
    [SerializeField] string levelDataFile = "levelData.json";
    [SerializeField] string cardJsonFile = "cardData.json";

    Card currentCard;
    Card previousCard;

    List<Card> cards; // cards used for current level
    List<Card> deck; // all cards from all levels, loaded once
    public List<LevelState> levelStates;


    public void Register(Card card) => cards.Add(card);


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




        //StartCoroutine(FaceUpAllCards(true, 0, true));
        //StartCoroutine(FaceUpAllCards(false, 2, true));
    }

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
        foreach (var card in cards) card.SetState(Card.State.FaceUp);
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
