using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System.Linq;

public class CreateCards : MonoBehaviour
{
    static int numberOfColumns = 5;
    static int numberOfRows = 4;
    static int numberOfGroups = 2; //2 in game "memory"
    public Card[,] cards = new Card[numberOfRows, numberOfColumns];
    public Vector3 inBetweenSpace = new Vector3(0.0f, 0.0f, 0.0f);
    List<Vector2> turnedCards = new List<Vector2>(); // order and which cards have been turned
    Card currentCard = new Card();
    Card previousCard = new Card();
    int numberOfTurns = 0;
    UnityEvent m_MyEvent;

    /*
     Todo: start with all cards facing up and turning down
     when it's finished, show a "congratulations" menu and let the player "play again"
     add a user/password menu

     */

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("initializing cards...");
        InitCards();

        StartCoroutine(FaceUpAllCards(true, 0, true));
        StartCoroutine(FaceUpAllCards(false, 2, true));

    }

    // the card class only knows about a card and it's state
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
            Uninitialized = 3
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

    private Vector3 IndexToCardCoordinate(Vector3 ijk, Vector3 cardDimensions, Vector3 betweenSpace)
    {
        Vector3 vec = new Vector3(0, 0, 0);
        vec.x = ijk.x * (cardDimensions.x + betweenSpace.x);
        vec.y = ijk.y * (cardDimensions.y + betweenSpace.y);
        vec.z = ijk.z * (cardDimensions.z + betweenSpace.z);

        return vec;
    }

    // Find the board and places cards randomly
    public void InitCards()
    {
        int tot = numberOfRows * numberOfColumns;
        int indexTotal = tot % numberOfGroups;
        int number = 0;
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
        GameObject the_card = GameObject.Find("the_card");
        GameObject master_card_back = GameObject.Find("master_card_back");
        GameObject canvas = GameObject.Find("Canvas");
        GameObject background = GameObject.Find("Background");

        var cardDimensions = master_card_back.GetComponent<Renderer>().bounds.size;

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

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                GameObject go;
                go = (GameObject)Instantiate(the_card);
                go.name = "card_" + i + "_" + j;
                Material mm = new Material(Shader.Find("Unlit/Texture"));
                //offset card with scale/2, because pivot point is center of card
                go.transform.position = new Vector2(250, 100) + new Vector2(j * cardDimensions.x * 1.1f, i * cardDimensions.y *1.1f);
                go.transform.SetParent(card_entity.transform);
                string filename = "card textures/flags_1.1/" + GetCardFromDeck(i, j);
                Texture2D tex = Resources.Load<Texture2D>(filename);
                mm.mainTexture = tex;

                Transform child = go.transform.GetChild(0);
                child.GetComponent<Renderer>().material = mm;
            }
        }

        card_entity.transform.position = new Vector2(0.0f, 0.0f);
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

    // returns the number of turned cards
    private int TurnedCards()
    {
        int sum = 0;
        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                Card.State s = cards[i, j].GetState();
                if (s == Card.State.FaceUp)
                {
                    sum++;
                }
            }
        }
        return sum;
    }

    private IEnumerator FaceUpAllCards(bool faceUp, int time, bool doesNotMatterWhatState)
    {
        Card.State toState = faceUp ? Card.State.FaceDown: Card.State.FaceUp;
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
                    var parpar = go;
                    Animation anim = parpar.GetComponent<Animation>();
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
        int numberOfFinishedCards = 0;
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

    // Update is called once per frame
    void Update()
    {
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
                    var test2 = hit.collider.gameObject;
                    var parpar = test2.transform.parent;
                    Animation anim = parpar.GetComponent<Animation>();
                    Vector2 vv = GetCardFromGameObject(parpar.name);
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

                        if (numberOfTurns==2)
                        {
                            StartCoroutine(FaceUpAllCards(false, 1, false));
                        }
                    }
                }
            }
        }
    }
}
