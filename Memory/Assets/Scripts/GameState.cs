using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameNamespace;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.XR;

/* This class contains the game state of the game, level data, bonuses and penalties et cetera */
// add state for menu, pause, running and end, replay
public class GameState
{
    public int stage; // current stage, reference levelState
    //public int numberOfTurns; // 0, 1 or 2
    public float startTime; // used for animation

    // TODO: check if we need to serialize:
    [Header("Level & asset files")]
    [SerializeField] string levelDataFile = "levelData.json";
    [SerializeField] string cardJsonFile = "cardData.json";

    // We use cardobject so we can access the card data and the gameobject at the same time for bookkeeping (gameState) but also animation (gameObject)
    CardObject currentCardObject;
    CardObject previousCardObject;

    public List<Card> deck; // all cards from all levels, loaded once
    public List<CardObject> cards; // cards used for current level with gameobjects attached to each card
    public List<LevelState> levelStates;
    private List<bool> matchHistory; // keep a record of the matching cards, used for bonuses and penalties

    private static GameState _instance;
    public static GameState Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameState();
            return _instance;
        }
    }


    private GameState()
    {
        startTime = Time.realtimeSinceStartup;
        stage = 0;

        //TODO: add a high scores JSON?
        string cardDataFilename = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, cardJsonFile));
        string levelDataFilename = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, levelDataFile));

        deck = JsonConvert.DeserializeObject<List<Card>>(cardDataFilename);
        levelStates = JsonConvert.DeserializeObject<List<LevelState>>(levelDataFilename);
        LevelState level = levelStates[stage];

        cards = Enumerable.Repeat<CardObject>(null, level.Rows * level.Columns).ToList();
        matchHistory = new List<bool>();
    }

    // Button related code
    readonly HashSet<GameNamespace.BonusType> available = new HashSet<GameNamespace.BonusType>();
    public  event Action OnBonusAvailabilityChanged;

    public bool IsBonusAvailable(GameNamespace.BonusType bonus)
        => available.Contains(bonus);

    public void SetBonusAvailable(GameNamespace.BonusType bonus, bool bonusAvailable)
    {
        if (bonusAvailable) available.Add(bonus);
        else available.Remove(bonus);
        OnBonusAvailabilityChanged?.Invoke();
    }

    public void GrantBonus(GameNamespace.BonusType bonus)
    {
        if(bonus== GameNamespace.BonusType.Wind)
        {
            _instance.Wind();

        }
        if(bonus== GameNamespace.BonusType.Idea)
        {
            _instance.Idea();
           
            //TODO. fix lamp trigger
        }

        //remove the bonus from the available bonuses
        SetBonusAvailable(bonus, false);
    }


    public void AddCard(CardObject co, int i)
    {
        cards[i] = co;
    }


    public void StartCurrentStage()
    {
        ShowAllCards();
    }

    public void CardClicked(int cardIndex)
    {
        // get index from cardObject not the index in "cards"
        CardObject cardObject = (CardObject)cards.Where(x => x.Data.Index == cardIndex).First();

        // TODO: dont' allow a flip until the current animation is finished, check the state
        if (cardObject.Data.CurrentState == Card.State.Finished)
        {
            return;
        }

        cardObject.Data.SetState(Card.State.FaceUp);

        // first card clicked
        if (currentCardObject == null)
        {
            currentCardObject = cardObject;
            var currentCardInteraction = currentCardObject.View.transform.GetChild(0).GetComponent<Interaction>();
            currentCardInteraction.FlipCard(true);
            currentCardObject.Data.SetState(Card.State.FaceUp);

            return;
        }
        // second card clicked
        if (previousCardObject == null)
        {
            previousCardObject = currentCardObject;
            currentCardObject = cardObject;
            currentCardObject.Data.SetState(Card.State.FaceUp);

            var currentCardInteraction = currentCardObject.View.transform.GetChild(0).GetComponent<Interaction>();
            var previousCardInteraction = previousCardObject.View.transform.GetChild(0).GetComponent<Interaction>();

            currentCardInteraction.FlipCard(true);

            // Matching cards
            if (currentCardObject.Data.GroupIndex == previousCardObject.Data.GroupIndex)
            {
                // do something, like remove the cards or mark them as matched
                previousCardObject.Data.SetState(Card.State.Finished);
                currentCardObject.Data.SetState(Card.State.Finished);
                previousCardObject = null; // reset previous card
                matchHistory.Add(true);
            }
            else
            {
                // flip these cards back down
                currentCardObject.Data.SetState(Card.State.FaceDown);
                previousCardObject.Data.SetState(Card.State.FaceDown);

                CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.DelayedFlipCard(false, 1));
                CoroutineRunner.Instance.RunCoroutine(previousCardInteraction.DelayedFlipCard(false, 1));

                previousCardObject = null;
                currentCardObject = null;
                matchHistory.Add(false);
            }

            //TODO: if the two last matchings are true, then give the lamp bonus
            if (LastTwoAreTrue(matchHistory))
            {
                GameObject cardsGO = GameObject.Find("Cards");
                CreateCards _cards = cardsGO.GetComponent<CreateCards>();

                GameState.Instance.SetBonusAvailable(GameNamespace.BonusType.Idea, true);

                matchHistory.Add(false); //pad with false, otherwise if the next is matching, we will get another pair of matching cards
            }
        }


        // TODO: check how many cards are totally turned over
        if (IsLevelDone())
        {
            // TODO: Show stats and then change to new level
        }

        previousCardObject = null;
        currentCardObject = null;
    }

    public void Click()
    {
        //Debug.Log("Click!!");

    }

    // check list of bools if the last two elements are true (for matchHistory)
    bool LastTwoAreTrue(IList<bool>? list)
    {
        return list != null &&
               list.Count >= 2 &&
               list.TakeLast(2).All(v => v);
    }

    // Turn all the cards to show them for a short time
    public void ShowAllCards()
    {
        foreach (CardObject card in cards)
        {
            var currentCardInteraction = card.View.transform.GetChild(0).GetComponent<Interaction>();
            CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.DelayedFlipCard(true, 0));
        }
        foreach (CardObject card in cards)
        {
            var currentCardInteraction = card.View.transform.GetChild(0).GetComponent<Interaction>();
            CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.DelayedFlipCard(false, 3));
        }
    }

    public bool IsLevelDone()
    {
        return cards.All(item => item.Data.CurrentState == Card.State.Finished);
    }

  
    // Reveals the cards by lighting a light bulb.
    void Idea()
    {
        //TODO: make this neater
        GameObject cardsGO = GameObject.Find("Cards");
        CreateCards _cards = cardsGO.GetComponent<CreateCards>();
        _cards.TriggerLamp();

    }

    // Spins all cards a couple turns
    // requires the cards the themselves have knowledge about their position
    void Wind()
    {
        foreach (CardObject card in cards)
        {
            var currentCardInteraction = card.View.transform.GetChild(0).GetComponent<Interaction>();
            CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.SpinCard(2, 10));
        }
    }

    void NextLevel()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
