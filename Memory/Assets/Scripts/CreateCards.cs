using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

/*
 Responsible to instantiate the cards for each stage.
 */
public class CreateCards : MonoBehaviour
{
    Bounds cardBounds; // defined by the "cloud" gameObject
    GameObject lightBulb;
    GameObject cards__;
    int OnID = Shader.PropertyToID("_on"); // light bulb
    int LitID = Shader.PropertyToID("_lit"); // card
    int BulbTransparencyID = Shader.PropertyToID("_transparency");

    public void Awake()
    {
        //var gameState = GameState.Instance;
        //TODO: can we call something else here that just starts the game, stores the gameobject of the createcards and proceeds with the game?
        //GameState.Instance.InitializeDeck(this.gameObject);
        GameState.Initialize(this);
    }

    public List<CardObject> BuildBoard(LevelState level, List<Card> deck)
    {
        List<CardObject> createdCards = new List<CardObject>();
        cardBounds = Misc.GetBounds("Cloud");
        lightBulb = GameObject.Find("Light bulb");
        cards__ = GameObject.Find("master_card"); // master_card or Cards
        GameObject cards_ = GameObject.Find("Cards");
        GameObject card_ = GameObject.Find("Card");

        int rows_ = level.Rows;
        int cols_ = level.Columns;

        float width = cardBounds.max.x - cardBounds.min.x;
        float height = cardBounds.max.y - cardBounds.min.y;

        if (width == 0)
        {
            Debug.LogError("Card width is zero. Check the prefab’s renderer.");
            return null;
        }

        Vector2 origin = new(cardBounds.min.x + 0.2f * width, cardBounds.min.y);
        //GameObject canvas = GameObject.Find("Canvas");

        float scaling = Mathf.Max(width, height) / Mathf.Max(rows_ + 1, cols_ + 1);
        string filename;
        MaterialPropertyBlock mpb;
        int frontTexPropId;
        Texture2D tex;
        Interaction currentCardInteraction;
        Renderer rend;
        Shader cardShader;

        CardObject cardObject;
        GameObject go;

        CardIndex ci;

        for (int i = 0; i < cols_ * rows_; i++)
        {
            int jj = i % cols_; // column
            int ii = i / cols_; //row

            go = Instantiate(card_);
            ci = go.AddComponent<CardIndex>();
            ci.index = i;

            currentCardInteraction = go.transform.GetChild(0).GetComponent<Interaction>(); // the_card has the interaction script
            go.transform.localScale = Vector3.one * 0.8f;
            go.transform.SetParent(cards_.transform, false);
            // TODO. put in a concentric pattern instead
            // given the outer limits of the cloud, scale the cards to fit them being placed concentrically/spiral
            go.transform.localPosition = new Vector3(
                    jj,
                    ii,
                    0f);

            go.transform.parent.transform.localScale = new Vector3(scaling, scaling, scaling);
            cardShader = Shader.Find("Shader Graphs/Card");
            frontTexPropId = Shader.PropertyToID("_FrontTexture");
            filename = deck[i].Path.Replace(".png", "");
            //TODO: check if filename exist!
            tex = Resources.Load<Texture2D>(filename);
            rend = go.GetComponentInChildren<Renderer>(); // the one that’s already there
            mpb = new MaterialPropertyBlock();
            mpb.SetTexture(frontTexPropId, tex);
            mpb.SetFloat(BulbTransparencyID, 0f);
            mpb.SetFloat(LitID, 0f);
            rend.SetPropertyBlock(mpb);
            cardObject = new CardObject(deck[i], go);
            cardObject.Data.SetState(Card.State.FaceDown);
            cardObject.Data.Index = ci.index; // store index of card
            createdCards.Add(cardObject);
        }


        //cards_.transform.parent.transform.localScale = new Vector3(scaling, scaling, scaling);
        cards_.transform.position = origin;
        Misc.Randomize(createdCards); // TODO: doesn't seem to work, fix
        return createdCards;
    }

   
    // TODO: create a new monobehavior for the UI animations, tie in to tumbleweed movement?
    public void TriggerLamp()
    {
        StartCoroutine(FlickerThenFade());
    }

    // would be nicer if this function is located in another persistant class so we dont have to fetch the bulb every time
    // TODO: new to fix
    private IEnumerator FlickerThenFade()
    {
        Material _bulbMat = lightBulb.GetComponent<Renderer>().material;
        Material card_mat = cards__.GetComponent<Renderer>().material;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        float elapsed = 0f;

        for (float t = 0f; t < 1f; t += Time.deltaTime)
        {
            float v = Mathf.Lerp(0f, 1f, t);

            lightBulb.GetComponentInChildren<Renderer>().GetPropertyBlock(mpb);
            _bulbMat.SetFloat(BulbTransparencyID, v);
        }

        // TODO: check that the cards are updated properly
        while (elapsed < 3f)
        {
            float onOff = Random.value < 0.5f ? 1f : 0f;

            foreach (var cardObj in GameState.Instance.cards)
            {
                Renderer rend = cardObj.View.GetComponentInChildren<Renderer>();
                rend.GetPropertyBlock(mpb);
                mpb.SetFloat(LitID, onOff * 0.566f);
                rend.SetPropertyBlock(mpb);
            }

            _bulbMat.SetFloat(OnID, onOff);
            float wait = Random.Range(0.05f, 0.15f);

            yield return new WaitForSeconds(wait);
            elapsed += wait;

            _bulbMat.SetFloat(OnID, 0f);

            foreach (var cardObj in GameState.Instance.cards) //TODO: replace this
            {
                Renderer rend = cardObj.View.GetComponentInChildren<Renderer>();
                rend.GetPropertyBlock(mpb);
                mpb.SetFloat(LitID, 0f);
                rend.SetPropertyBlock(mpb);
            }
        }

        _bulbMat.SetFloat(BulbTransparencyID, 0f);
    }
}
