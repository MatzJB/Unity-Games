using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameNamespace;
using Newtonsoft.Json;
using UnityEngine;

/* This class contains the game state of the game, level data, bonuses and penalties et cetera */
// add state for menu, pause, running and end, replay

public sealed class GameState
{
    private static GameState _instance;
    public static GameState Instance => _instance ?? throw new Exception("GameState not initialized");

    private readonly CreateCards _script;

    public int stage; // current stage, reference levelState
    public float startTime; // used for animation

    // TODO: check if we need to serialize:
    private readonly string levelDataFile = "levelData2.json";
    private readonly string cardJsonFile = "cardData2.json";

    // We use cardobject so we can access the card data and the gameobject at the same time for bookkeeping (gameState) but also animation (gameObject)
    public CardObject currentCardObject;
    public CardObject previousCardObject;
    // in the editor I want to start from stage 1 each time
    public List<Card> deck; // all cards from all levels, loaded once
    public List<CardObject> cards; // cards loaded each level
    public List<LevelState> levelStates;
    private List<bool> matchHistory; // keep a record of the matching cards, used for bonuses and penalties

    private GameState(CreateCards script)
    {
        _script = script;
        stage = -1;
        startTime = Time.time;
        deck = new List<Card>();
        cards = new List<CardObject>();
        levelStates = new List<LevelState>();
        matchHistory = new List<bool>();
        LoadLevelData(levelDataFile);
        LoadCardData(cardJsonFile);
        startTime = Time.realtimeSinceStartup;
    }

    public static void Initialize(CreateCards script)
    {
        if (_instance == null)
        {
            _instance = new GameState(script);
            _instance.LoadNextStage();
        }
    }

    private void LoadLevelData(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string levelData = File.ReadAllText(filePath);
        levelStates = JsonConvert.DeserializeObject<List<LevelState>>(levelData);
    }

    private void LoadCardData(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string cardData = File.ReadAllText(filePath);
        deck = JsonConvert.DeserializeObject<List<Card>>(cardData);
    }

    private void PopulateLevel(int stage)
    {
        //TODO: when we are at the end, stop the game
        LevelState level = levelStates[stage];
        cards = Enumerable.Repeat<CardObject>(null, level.Rows * level.Columns).ToList();
        matchHistory = new List<bool>();
    }

    private void LoadNextStage()
    {
        DestroyCards();
        AdvanceStage();
        PopulateLevel(stage);

        LevelState level = levelStates[stage];
        CreateCards createCards = _script.GetComponent<CreateCards>();

        if (createCards != null)
        {
            // TODO: something is wrong here, fix
            List<Card> filteredDeck = deck.Where(card => card.Category == level.Category).ToList();
            cards = createCards.BuildBoard(level, filteredDeck);
        }
        else
        {
            Debug.LogError("CreateCards component not found on the provided GameObject.");
        }
        StartCurrentStage();
    }

    // TODO: check how this works
    public void DestroyCards()
    {
        foreach (CardObject card in cards)
        {
            if (card != null && card.View != null)
            {
                GameObject.Destroy(card.View);
            }
        }
        cards.Clear();
    }

    public event System.Action<string> OnStageTextChanged;

    string _stageText;
    public string StageText
    {
        get => _stageText;
        set
        {
            if (_stageText == value) return;
            _stageText = value;
            OnStageTextChanged?.Invoke(_stageText);
        }
    }

    public void AdvanceStage()
    {
        stage++;

        StageText = $"Stage {levelStates[stage].Stage}";
    }

    // Button related code
    readonly HashSet<BonusType> available = new HashSet<BonusType>();
    public event Action OnBonusAvailabilityChanged;

    public bool IsBonusAvailable(BonusType bonus)
        => available.Contains(bonus);

    public void SetBonusAvailable(BonusType bonus, bool bonusAvailable)
    {
        //Debug.Log($"adding bonus {bonus}");
        if (bonusAvailable) available.Add(bonus);
        else available.Remove(bonus);
        OnBonusAvailabilityChanged?.Invoke();
    }

    public void GrantBonus(BonusType bonus)
    {
        if (bonus == BonusType.Wind)
        {
            _instance.Wind();
        }
        if (bonus == BonusType.Idea)
        {
            _instance.Idea();
        }

        SetBonusAvailable(bonus, false);
    }


    public void AddCard(CardObject co, int i)
    {
        co.Data.SetState(Card.State.FaceDown);
        cards[i] = co;
    }

    public void StartCurrentStage()
    {
        ShowAllCards();

        if (cards.Any(item => item.Data.CurrentState == Card.State.Finished))
        {
            Debug.Log("Something is wrong because we just started the stage and at least one card is facing up");
        }
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
                previousCardObject.Data.SetState(Card.State.Finished);
                currentCardObject.Data.SetState(Card.State.Finished);
                previousCardObject = null; // reset previous card
                matchHistory.Add(true);
            }
            else
            {
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

                CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.Delay(2));

                if (IsBonusAvailable(BonusType.Idea))
                {
                    GameState.Instance.SetBonusAvailable(BonusType.Wind, true);
                }
                else
                {
                    GameState.Instance.SetBonusAvailable(BonusType.Idea, true);
                }

                matchHistory.Add(false); //pad with false, otherwise if the next is matching, we will get another pair of matching cards
            }
        }

        // TODO: check how many cards are totally turned over
        if (IsLevelDone())
        {
            //TODO: is there a better way to do this?
            //TODO:  cache cardsGO (and _cards?)
            GameObject cardsGO = GameObject.Find("Cards");
            CreateCards _cards = cardsGO.GetComponent<CreateCards>();

            var currentCardInteraction = currentCardObject.View.transform.GetChild(0).GetComponent<Interaction>();

            //TODO: add a intro text, some pausing and then advance to the next stage
            CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.Delay(2));

            LoadNextStage();
        }

        previousCardObject = null;
        currentCardObject = null;
    }


    // check list of bools if the last two elements are true (for matchHistory)
    bool LastTwoAreTrue(IList<bool> list)
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
            CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.DelayedFlipCard(false, 2));
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
        // cache if possible, like I said above
        GameObject cardsGO = GameObject.Find("Cards");
        CreateCards _cards = cardsGO.GetComponent<CreateCards>();
        _cards.TriggerLamp();
    }

    void Wind()
    {
        foreach (CardObject card in cards)
        {
            var currentCardInteraction = card.View.transform.GetChild(0).GetComponent<Interaction>();
            CoroutineRunner.Instance.RunCoroutine(currentCardInteraction.SpinCard(2, 10));
        }
    }
}
