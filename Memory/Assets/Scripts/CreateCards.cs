using System.Collections.Generic;
using UnityEngine;
using Assets;
using Unity.Android.Gradle.Manifest;
using System.Diagnostics;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using System.Collections;
using System;
using NUnit.Framework;
using static Assets.Misc;
using System;
using System.Collections.Generic;

// TODO: refactor this file, move content from createCards to Interaction.cs, Card.cs,...
// Question: what happens if I switch stage, can I do that in real time in the editor?

public class CreateCards : MonoBehaviour
{
    float startTime;
    GameObject bulb;
    Renderer bulbRenderer;
    GameObject cloud;
    Bounds cardBounds; // defined by the "cloud" gameObject

    //GameObject card_;
    public GameState gameState;

    void Start()
    {
        gameState = new GameState();
        UnityEngine.Debug.Log("gamestate " + gameState);
        UnityEngine.Debug.Assert(gameState != null, "Failed to add GameState component");
        UnityEngine.Debug.Log("Creating GameState object");

        cloud = GameObject.Find("Cloud");
        cardBounds = Misc.GetBounds("Cloud");

        InitCards();
        startTime = Time.realtimeSinceStartup;

        bulb = GameObject.Find("Light bulb");
        if (bulb == null) { UnityEngine.Debug.LogError("Light bulb not found"); return; }
        bulbRenderer = bulb.GetComponent<Renderer>();
    }


    public void Awake()
    {
      

    }

    // Build gameObjects, register with gameState.cards
    public void BuildBoard()
    {
        UnityEngine.Debug.Log("buildboard");

        LevelState level = gameState.levelStates[gameState.stage];

        int rows_ = level.Rows;
        int cols_ = level.Columns;
        
        GameObject cards_ = GameObject.Find("Cards");
        GameObject card_ = GameObject.Find("Card");


        //Bounds cardBounds = card_.GetComponentInChildren<Renderer>().bounds;

        //float width = cardBounds.size.x;
        //float height = cardBounds.size.y;

        float width = cardBounds.max.x - cardBounds.min.x;
        float height = cardBounds.max.y - cardBounds.min.y;

        if (width == 0)
        {
            UnityEngine.Debug.LogError("Card width is zero. Check the prefab’s renderer.");
            return;
        }

        Vector2 origin = new(cardBounds.min.x + 0.2f * width, cardBounds.min.y);
        //float scale = Mathf.Max(width, height) / Mathf.Max(rows + 1, cols + 1);

        //GameObject card_ = GameObject.Find("Card"); // for floating misaligned cards
        //GameObject the_card = GameObject.Find("the_card"); //animation
        //GameObject master_card = GameObject.Find("master_card");
        GameObject canvas = GameObject.Find("Canvas");



        //if (width == 0)
        //    Debug.LogError("Card width is zero, check the card prefab and its renderer");
        float scaling = Mathf.Max(width, height) / Mathf.Max(rows_ + 1, cols_ + 1);

        for (int i = 0; i < cols_ * rows_; i++)
        {
            int jj = i % cols_; // column
            int ii = i / cols_; //row

            GameObject go = Instantiate(card_);
            CardIndex ci = go.AddComponent<CardIndex>();
            ci.index = i;
            //var currentCardInteraction = go.View.GetComponent<Interaction>();

            var currentCardInteraction = go.transform.GetChild(0).GetComponent<Interaction>(); // the_card has the interaction script
            currentCardInteraction.Init(gameState);
            UnityEngine.Debug.Log($" cardindex added {i}");
            
            // important because Interaction is in the parent, and we want these two to be close by
            //go..AddComponent<CardIndex>().index = i;
            go.transform.localScale = Vector3.one * 0.8f;
            go.transform.SetParent(cards_.transform, false);
            //be careful because Interaction.cs scales back to 1 because of hovering effect... maybe there is a better way?

            go.transform.localPosition = new Vector3(
                    jj,
                    ii,
                    0f);

            go.transform.parent.transform.localScale = new Vector3(scaling, scaling, scaling);
            //z scaling must be scaling otherwise it will be flat

            //        0f);
            //go.transform.localPosition = new Vector3(
            //        scaling * jj,
            //        scaling * ii,
            //        0f);

            //var card = go.GetComponent<Card>(); // get the script
            //card.Init(index, gameState); // init card
            //gameobject and card doesn't have to be connected more than with the gameObject, the card does not communicate back to the 3d object
            //the data object "card" is created in gamestate before this...
            // get it from the decka and assign here
            //go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            Shader cardShader = Shader.Find("Shader Graphs/Card");
            int frontTexPropId = Shader.PropertyToID("_FrontTexture");
            string filename = gameState.deck[i].Path.Replace(".png", "");
            Texture2D tex = Resources.Load<Texture2D>(filename);
            Renderer rend = go.GetComponentInChildren<Renderer>(); // the one that’s already there
            var mpb = new MaterialPropertyBlock();
            mpb.SetTexture(frontTexPropId, tex);
            rend.SetPropertyBlock(mpb);
            CardObject tmp = new CardObject(gameState.deck[i], go);
            tmp.Data.Index = i;
            // add cards using index because otherwise we cannot randomize them
            gameState.AddCard(tmp, i);
            //var card = go.GetComponent<Card>(); // get the script?

        }

        //float scaling = Mathf.Max(width, height) / Mathf.Max(rows_ + 1, cols_ + 1);
        //cards_.transform.localScale = new Vector3(scaling, scaling, scaling);//z scaling must be scaling otherwise it will be flat

        cards_.transform.position = origin;
        Misc.Randomize(gameState.cards);
    }

    // Find the board and places cards randomly
    public void InitCards() // load level
    {
        LevelState levelData = gameState.levelStates[gameState.stage]; 

        //Assets.Misc.Randomize(cards);
        GameObject cards_ = GameObject.Find("Cards");
        GameObject card_ = GameObject.Find("Card"); // for floating misaligned cards
        GameObject the_card = GameObject.Find("the_card"); //animation
        GameObject master_card = GameObject.Find("master_card");
        GameObject canvas = GameObject.Find("Canvas");
        //GameObject background = GameObject.Find("Background");

        BuildBoard();
        gameState.StartCurrentStage();

        //var cardDimensions = master_card.GetComponent<Renderer>().bounds.size;

        //RectTransform rt = canvas.GetComponent<RectTransform>();
        //Vector2 canvasWidthHeight = new Vector2(rt.rect.width, rt.rect.height);

        //float wTotal = canvasWidthHeight.x / numberOfColumns;
        //float hTotal = canvasWidthHeight.y / numberOfRows;
        ////Vector2 inBetweenSpace = new Vector2(1, 1); //space between cards
        //float cardWidthTarget = (wTotal - numberOfColumns * inBetweenSpace.x) / (numberOfColumns);
        //float cardHeightTarget = (hTotal - numberOfRows * inBetweenSpace.y) / (numberOfRows);
        //float cardScaleX = cardWidthTarget / cardDimensions.x;
        //float cardScaleY = cardHeightTarget / cardDimensions.y;

        //Vector3 cardScale = new Vector3(cardScaleX, cardScaleY, 1);
        //float scale = Mathf.Max(cardScale.x, cardScale.y);
        //cardScale = new Vector3(scale, scale, scale);
        //var ww = numberOfColumns * cardWidthTarget;
        //Vector2 cardOffset = new Vector2(ww / 2, 0);
        //Vector3 tmp = new Vector3(cardScale.x, cardScale.y, 0);

        //float width = cardBounds.max.x - cardBounds.min.x;
        //float height = cardBounds.max.y - cardBounds.min.y;

        //if (width == 0)
        //    Debug.LogError("Card width is zero, check the card prefab and its renderer");

        //Vector2 cardsOrigin = new(cardBounds.min.x + 0.2f * width, cardBounds.min.y);

        //Debug.Log(" cards origin :" + cardsOrigin);
        //int rows = levelData.Rows;
        //int cols = levelData.Columns / levelData.CardsToMatch;
        //float countMax = Mathf.Max(rows, cols);
        //cards_.transform.position = new Vector3(0, 0, 0);
        //Shader cardShader = Shader.Find("Shader Graphs/Card");
        //int frontTexPropId = Shader.PropertyToID("_FrontTexture");
        //int i = 0;

        //foreach (var card in gameState.cards)
        //{
        //    GameObject cardObject = Instantiate(card_);
        //    cardObject.name = $"card";
        //    cardObject.transform.SetParent(cards_.transform, false);
        //    // place cards in a mesh grid
        //    int j = i / levelData.Columns;
        //    cardObject.transform.localPosition = new Vector3(j, i % rows, 0);
        //    cardObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        //    i++;


        //    // load texture for this slot
        //    //string path = $"card textures/flags_1.1/{GetCardFromDeck(i, j)}";

        //    //read from json

        //    Texture2D tex = Resources.Load<Texture2D>(card.path);

        //    Renderer rend = cardObject.GetComponentInChildren<Renderer>(); // the one that’s already there

        //    if (rend == null)
        //    {
        //        Debug.LogError($"{cardObject.name} has no renderer");
        //        continue;
        //    }

        //    // create one shared material for the whole deck (optional)
        //    if (rend.sharedMaterial == null || rend.sharedMaterial.shader != cardShader)
        //        rend.sharedMaterial = new Material(cardShader);
        //    cardObject.tag = "Card";

        //    var mpb = new MaterialPropertyBlock();
        //    mpb.SetTexture(frontTexPropId, tex);
        //    rend.SetPropertyBlock(mpb);
        //}


        //cards_.transform.position = cardsOrigin;
        //float scaling = Mathf.Max(width, height) / Mathf.Max(rows + 1, levelData.Columns + 1);
        //cards_.transform.localScale = new Vector3(scaling, scaling, scaling);//z scaling must be scaling otherwise it will be flat

        //card_.tag = "Untagged";

        //Destroy(card_);
    }

    // returns file name for card from a number
    //public string GetCardFromDeck(int i, int j)
    //{
    //    int groupID = cards[i, j].GroupIndex;
    //    //int index = cards[i, j].In;
    //    string[] flags = { "es", "cn", "us", "il", "it", "jp", "se", "fi", "ca", "ar" };
    //    string appendage = (groupID > 0) ? "_text" : "";
    //    string cardName = flags[index] + appendage;

    //    return cardName;
    //}

    //private Vector2 GetCardFromGameObject(string name)
    //{
    //    string[] parts = name.Split('_');
    //    int i = int.Parse(parts[1]);
    //    int j = int.Parse(parts[2]);

    //    return new Vector2(i, j);
    //}

  


    //private IEnumerator FaceUpAllCards(bool faceUp, int time, bool doesNotMatterWhatState)
    //{
    //    Card.State toState = faceUp ? Card.State.FaceDown : Card.State.FaceUp;
    //    Card.State toStateInverted = !faceUp ? Card.State.FaceDown : Card.State.FaceUp;

    //    yield return new WaitForSeconds(time);


    //    //for (int i = 0; i < numberOfRows; i++)
    //    //{
    //    //    for (int j = 0; j < numberOfColumns; j++)
    //    //    {
    //    foreach (var card in cards)
    //    {
    //        Card.State s = card.CurrentState;
    //        if (s == toState || doesNotMatterWhatState)
    //        {
    //            //find a way to attach object information to the 3d object
    //            //GameObject go = GameObject.Find("card_" + i + "_" + j);

    //            Transform childT = go.transform.GetChild(0);
    //            GameObject childGo = childT.gameObject;
    //            Animation anim = childGo.GetComponent<Animation>();

    //            if (faceUp)
    //                anim.Play("Flip");
    //            else
    //                anim.Play("FlipBack");

    //            cards[i, j].SetState(toStateInverted);
    //        }
    //    }
    //}
    //numberOfTurns = 0;
    //}

    //// sets each card to face down 
    //private void ResetCardStates()
    //{
    //    for (int i = 0; i < numberOfRows; i++)
    //    {
    //        for (int j = 0; j < numberOfColumns; j++)
    //        {
    //            cards[i, j].SetState(Card.State.FaceDown);
    //        }
    //    }
    //}

    //private bool GameOver()
    //{
    //    int numberOfFinishedCards = 0;
    //    for (int i = 0; i < numberOfRows; i++)
    //    {
    //        for (int j = 0; j < numberOfColumns; j++)
    //        {
    //            Card.State s = cards[i, j].GetState();

    //            if (s == Card.State.Finished)
    //            {
    //                numberOfFinishedCards++;
    //            }
    //            else
    //                return false;
    //        }
    //    }

    //    bool gameIsOver = numberOfColumns * numberOfRows == numberOfFinishedCards;
    //    return gameIsOver;
    //}


    // Update is called once per frame
    void Update()
{

//    //update gameObect in scene:
//    //update the position of each card each frame, to have them floating
//    float elapsedSeconds = Time.realtimeSinceStartup - startTime;
//    float y = Mathf.Cos(elapsedSeconds / 60) * 30;
//    float elapsedMs = elapsedSeconds * 1000f;

//    var cardObject = GameObject.FindGameObjectsWithTag("Card");

//    foreach (var card in cardObject)
//    {
//        // find parent, and get card_i_j
//        Vector2 vv = GetCardFromGameObject(card.name);

//        card.transform.position = new Vector3(
//            card.transform.position.x,
//           card.transform.position.y + 0.02f * Mathf.Cos(elapsedSeconds + vv.x % 5 + vv.y),
//            //make this value relative to the card height
//            card.transform.position.z
//        );


//        float angle = Mathf.Cos(vv.y + 2f * elapsedSeconds) * 5;
//        float distance = 0.5f;

//        int LitID = Shader.PropertyToID("_on");
//        Renderer rend = card.GetComponentInChildren<Renderer>();
//        Material mat = rend.material;
//        float lightBulbLight = bulbRenderer.sharedMaterial.GetFloat(LitID);
//        float onOff = lightBulbLight == 1 ? 1 : 0;

//        mat.SetFloat("_lit", onOff * distance);
//        card.transform.localEulerAngles = new Vector3(
//    0,
//    0,
//    angle
//);


    }


    //if (GameOver())
    //{
    //    StartCoroutine(FaceUpAllCards(false, 1, true));
    //    Assets.Misc.Randomize(cards);
    //    numberOfTurns = 0;
    //    ResetCardStates();
    //    StartCoroutine(FaceUpAllCards(true, 2, true));
    //    StartCoroutine(FaceUpAllCards(false, 4, true));
    //}

    }


