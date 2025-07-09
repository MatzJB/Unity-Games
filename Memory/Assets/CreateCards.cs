using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System.Linq;
using Assets;
using UnityEditor;
using static CreateCards;
using UnityEngine.UIElements;
using System;

//TODO: refactor this file, move content from createCards to Interaction.cs, Card.cs,...

public class CreateCards : MonoBehaviour
{
    // TODO: This data will be fetched from a "levelData.json"
    static int numberOfColumns = 5;
    static int numberOfRows = 4;
    static int numberOfGroups = 2;
    //2 in game "memory", did not implement a way to match k cards at a time, would be difficult for a player?

    public Card[,] cards = new Card[numberOfRows, numberOfColumns];
    public Vector3 inBetweenSpace = new Vector3(0.0f, 0.0f, 0.0f);
    Card currentCard = new Card();
    Card previousCard = new Card();
    int numberOfTurns = 0;
    float startTime;
    GameObject bulb;
    Renderer bulbRenderer;
    GameObject cloud;
    Bounds cardBounds; // defined by the "cloud" gameObject
   


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("initializing cards...");
        cloud = GameObject.Find("Cloud");
        cardBounds = GetCloudBounds();

        InitCards();
        startTime = Time.realtimeSinceStartup;

        StartCoroutine(FaceUpAllCards(true, 0, true));
        StartCoroutine(FaceUpAllCards(false, 2, true));

        bulb = GameObject.Find("Light bulb");
        if (bulb == null) { Debug.LogError("Light bulb not found"); return; }
        bulbRenderer = bulb.GetComponent<Renderer>();

    }

    // the card class only knows about a card and it's state
    //will be replace with cardAsset
    public class Card
    {

        public enum Type : ushort
        {
            Picture = 0,
            Media = 1,
        }
        public enum State : ushort
        {
            FaceUp = 0,
            FaceDown = 1,
            Finished = 2,
            Uninitialized = 3,
            Frozen = 4, // frozen means the card is not active, it cannot be turned over
        }

        private int groupIndex = -1;
        public int GroupIndex
        {
            get { return groupIndex; }
            set { groupIndex = value; }
        }
        private int cardIndex = -1;
        public int CardIndex
        {
            get { return cardIndex; }
            set { cardIndex = value; }
        }

        Type type;
        State state = Card.State.Uninitialized;


        public State GetState()
        {
            return this.state;
        }

        public void SetState(Card.State s)
        {
            state = s;
        }

        public Card()
        {
            this.cardIndex = -1;
            this.groupIndex = -1;
            this.state = Card.State.Uninitialized;
            this.type = Card.Type.Picture;
        }

        public Card(int cardIndex, int groupIndex, State s, Type t)
        {
            this.cardIndex = cardIndex;
            this.groupIndex = groupIndex;
            this.state = s;
            this.type = t;
        }
    }








    Bounds GetCloudBounds()
    {
        var cloud = GameObject.Find("Cloud");
        if (cloud == null)
        {
            Debug.LogError("No clouds were found");
            return new Bounds();
        }
        var rends = cloud.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0)
        {
            Debug.LogError("No Renderer on Cloud or its children");
            return new Bounds();
        }

        var bounds = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
            bounds.Encapsulate(rends[i].bounds);

        return bounds;
    }



    // Find the board and places cards randomly
    public void InitCards()
    {
        int tot = numberOfRows * numberOfColumns;
        int indexTotal = tot % numberOfGroups;
        //int number = 0;
        int groupIndex = -1;

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                int oo = i * numberOfColumns + j;
                int cardIndex = oo % (tot / numberOfGroups);
                int n = numberOfColumns * i + j;

                if (cardIndex == 0)
                {
                    groupIndex++;
                }
                cards[i, j] = new Card(cardIndex, groupIndex, Card.State.FaceDown, Card.Type.Picture);
            }
        }

        Assets.Misc.Randomize(cards);

        GameObject card_entity = GameObject.Find("Cards");
        GameObject card_ = GameObject.Find("Card"); // for floating misaligned cards
        GameObject the_card = GameObject.Find("the_card"); //animation
        GameObject master_card = GameObject.Find("master_card");
        GameObject canvas = GameObject.Find("Canvas");
        GameObject background = GameObject.Find("Background");

        var cardDimensions = master_card.GetComponent<Renderer>().bounds.size;

        RectTransform rt = canvas.GetComponent<RectTransform>();
        Vector2 canvasWidthHeight = new Vector2(rt.rect.width, rt.rect.height);

        float wTotal = canvasWidthHeight.x / numberOfColumns;
        float hTotal = canvasWidthHeight.y / numberOfRows;
        //Vector2 inBetweenSpace = new Vector2(1, 1); //space between cards
        float cardWidthTarget = (wTotal - numberOfColumns * inBetweenSpace.x) / (numberOfColumns);
        float cardHeightTarget = (hTotal - numberOfRows * inBetweenSpace.y) / (numberOfRows);
        float cardScaleX = cardWidthTarget / cardDimensions.x;
        float cardScaleY = cardHeightTarget / cardDimensions.y;

        Vector3 cardScale = new Vector3(cardScaleX, cardScaleY, 1);
        float scale = Mathf.Max(cardScale.x, cardScale.y);
        cardScale = new Vector3(scale, scale, scale);
        var ww = numberOfColumns * cardWidthTarget;
        Vector2 cardOffset = new Vector2(ww / 2, 0);
        Vector3 tmp = new Vector3(cardScale.x, cardScale.y, 0);

        float width = cardBounds.max.x - cardBounds.min.x;
        float height = cardBounds.max.y - cardBounds.min.y;
        if (width == 0)
            Debug.LogError("Card width is zero, check the card prefab and its renderer");
        Vector2 cardsOrigin = new(cardBounds.min.x + 0.2f * width, cardBounds.min.y);

        Debug.Log(" cards origin :" + cardsOrigin);

        float countMax = Mathf.Max(numberOfRows, numberOfColumns);
        card_entity.transform.position = new Vector3(0, 0, 0);
        Shader cardShader = Shader.Find("Shader Graphs/Card");
        int frontTexPropId = Shader.PropertyToID("_FrontTexture");

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                GameObject card = Instantiate(card_);
                card.name = $"card_{i}_{j}";
                card.transform.SetParent(card_entity.transform, false);
                card.transform.localPosition = new Vector3(j, i, 0);
                card.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

                // load texture for this slot
                string path = $"card textures/flags_1.1/{GetCardFromDeck(i, j)}";
                Texture2D tex = Resources.Load<Texture2D>(path);

                Renderer rend = card.GetComponentInChildren<Renderer>(); // the one that’s already there
                if (rend == null)
                {
                    Debug.LogError($"{card.name} has no renderer");
                    continue;
                }

                // create one shared material for the whole deck (optional)
                if (rend.sharedMaterial == null || rend.sharedMaterial.shader != cardShader)
                    rend.sharedMaterial = new Material(cardShader);
                card.tag = "Card";

                var mpb = new MaterialPropertyBlock();
                mpb.SetTexture(frontTexPropId, tex);
                rend.SetPropertyBlock(mpb);
            }
        }

        card_entity.transform.position = cardsOrigin;
        float scaling = Mathf.Max(width, height) / Mathf.Max(numberOfRows + 1, numberOfColumns + 1);
        card_entity.transform.localScale = new Vector3(scaling, scaling, scaling);//z scaling must be scaling otherwise it will be flat

        card_.tag = "Untagged";

        //Destroy(card_);
    }

    // returns file name for card from a number
    public string GetCardFromDeck(int i, int j)
    {
        int groupID = cards[i, j].GroupIndex;
        int index = cards[i, j].CardIndex;
        string[] flags = { "es", "cn", "us", "il", "it", "jp", "se", "fi", "ca", "ar" };
        string appendage = (groupID > 0) ? "_text" : "";
        string cardName = flags[index] + appendage;

        return cardName;
    }

    private Vector2 GetCardFromGameObject(string name)
    {
        string[] parts = name.Split('_');
        int i = int.Parse(parts[1]);
        int j = int.Parse(parts[2]);

        return new Vector2(i, j);
    }

    // check if all turned up should be turned over back
    private bool CheckIfTurnIsOver()
    {
        //if 4 cards are turned over => turn all cards that are not dead over
        return false;
    }

    

    private IEnumerator FaceUpAllCards(bool faceUp, int time, bool doesNotMatterWhatState)
    {
        Card.State toState = faceUp ? Card.State.FaceDown : Card.State.FaceUp;
        Card.State toStateInverted = !faceUp ? Card.State.FaceDown : Card.State.FaceUp;

        yield return new WaitForSeconds(time);
        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                Card.State s = cards[i, j].GetState();
                if (s == toState || doesNotMatterWhatState)
                {
                    //find a way to attach object information to the 3d object
                    GameObject go = GameObject.Find("card_" + i + "_" + j);
                    Transform childT = go.transform.GetChild(0);
                    GameObject childGo = childT.gameObject;
                    Animation anim = childGo.GetComponent<Animation>();

                    if (faceUp)
                        anim.Play("Flip");
                    else
                        anim.Play("FlipBack");

                    cards[i, j].SetState(toStateInverted);
                }
            }
        }
        numberOfTurns = 0;
    }

    // sets each card to face down 
    private void ResetCardStates()
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                cards[i, j].SetState(Card.State.FaceDown);
            }
        }
    }

    private bool GameOver()
    {
        int numberOfFinishedCards = 0;
        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                Card.State s = cards[i, j].GetState();

                if (s == Card.State.Finished)
                {
                    numberOfFinishedCards++;
                }
                else
                    return false;
            }
        }

        bool gameIsOver = numberOfColumns * numberOfRows == numberOfFinishedCards;
        return gameIsOver;
    }

    // Spins all cards a couple turns
    // requires the cards the themselves have knowledge about their position
    void Wind()
    {


    }


    // Rotates the cards around the center of the cloud
    void Tornado()
    {



    }

    // Reveals the cards by lighting a light bulb
    void Idea()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //update gameObect in scene:
        //update the position of each card each frame, to have them floating
        float elapsedSeconds = Time.realtimeSinceStartup - startTime;
        float y = Mathf.Cos(elapsedSeconds / 60) * 30;
        float elapsedMs = elapsedSeconds * 1000f;

        var cardObject = GameObject.FindGameObjectsWithTag("Card");

        foreach (var card in cardObject)
        {
            // find parent, and get card_i_j
            Vector2 vv = GetCardFromGameObject(card.name);

            card.transform.position = new Vector3(
                card.transform.position.x,
               card.transform.position.y + 0.02f * Mathf.Cos(elapsedSeconds + vv.x % 5 + vv.y),
               //make this value relative to the card height
                card.transform.position.z
            );


            float angle = Mathf.Cos(vv.y + 2f * elapsedSeconds) * 5;
            float distance = 0.5f;

            int LitID = Shader.PropertyToID("_on");
            Renderer rend = card.GetComponentInChildren<Renderer>();
            Material mat = rend.material;
            float lightBulbLight = bulbRenderer.sharedMaterial.GetFloat(LitID);
            float onOff = lightBulbLight == 1 ? 1 : 0;

            mat.SetFloat("_lit", onOff * distance);
            card.transform.localEulerAngles = new Vector3(
        0,
        0,
        angle
    );
        }


        if (GameOver())
        {
            StartCoroutine(FaceUpAllCards(false, 1, true));
            Assets.Misc.Randomize(cards);
            numberOfTurns = 0;
            ResetCardStates();
            StartCoroutine(FaceUpAllCards(true, 2, true));
            StartCoroutine(FaceUpAllCards(false, 4, true));
        }

        if (numberOfTurns < 2) //we cannot accept inputs unless the number of turns are less than 2
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.collider.gameObject.transform.parent.CompareTag("Card"))
                    {
                        return;
                    }

                    var go = hit.collider.gameObject;
                    var parpar = go.transform.parent;

                    var parparpar = parpar.transform.parent;
                    string card_index = parpar.name;
                    Debug.Log(card_index);

                    Animation anim = go.GetComponent<Animation>();
                    Vector2 vv = GetCardFromGameObject(card_index);
                    currentCard = cards[(int)vv.x, (int)vv.y];

                    if (currentCard.GetState() == Card.State.FaceDown)
                    {
                        currentCard.SetState(Card.State.FaceUp);

                        if (numberOfTurns == 1) // turn is over
                        {
                            if (previousCard.CardIndex == currentCard.CardIndex) // matching cards
                            {
                                previousCard.SetState(Card.State.Finished);
                                currentCard.SetState(Card.State.Finished);
                            }
                        }
                        else
                        {
                            previousCard = currentCard;
                        }

                        anim.Play("Flip");
                        numberOfTurns++;

                        if (numberOfTurns == 2)
                        {
                            StartCoroutine(FaceUpAllCards(false, 1, false));
                        }
                    }
                }
            }
        }
    }
}
