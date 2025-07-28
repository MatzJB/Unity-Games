using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

/*
 Responsible to instantiate the cards.
 */
public class CreateCards : MonoBehaviour
{
    private GameObject cloud;
    Bounds cardBounds; // defined by the "cloud" gameObject
    GameObject lightBulb;
    GameObject cards__;
    int OnID = Shader.PropertyToID("_on"); // light bulb
    int LitID = Shader.PropertyToID("_lit"); // card
    int BulbTransparencyID = Shader.PropertyToID("_transparency");

    public void Awake()
    {
        var gameState = GameState.Instance;

        //gameState.InitializeDeck();
    }

    public List<CardObject> BuildBoard(LevelState level, List<Card> deck)
    {
        List<CardObject> createdCards = new List<CardObject>();

        cloud = GameObject.Find("Cloud");
        cardBounds = Misc.GetBounds("Cloud");
        lightBulb = GameObject.Find("Light bulb");

        //GameState.Instance.StartCurrentStage();

        cards__ = GameObject.Find("master_card"); //master_card or Cards

        int rows_ = level.Rows;
        int cols_ = level.Columns;

        GameObject cards_ = GameObject.Find("Cards");
        GameObject card_ = GameObject.Find("Card");

        float width = cardBounds.max.x - cardBounds.min.x;
        float height = cardBounds.max.y - cardBounds.min.y;

        if (width == 0)
        {
            Debug.LogError("Card width is zero. Check the prefab’s renderer.");
            return null;
        }

        Vector2 origin = new(cardBounds.min.x + 0.2f * width, cardBounds.min.y);
        GameObject canvas = GameObject.Find("Canvas");

        float scaling = Mathf.Max(width, height) / Mathf.Max(rows_ + 1, cols_ + 1);

        for (int i = 0; i < cols_ * rows_; i++)
        {
            int jj = i % cols_; // column
            int ii = i / cols_; //row

            GameObject go = Instantiate(card_);
            CardIndex ci = go.AddComponent<CardIndex>();
            ci.index = i;

            var currentCardInteraction = go.transform.GetChild(0).GetComponent<Interaction>(); // the_card has the interaction script
            go.transform.localScale = Vector3.one * 0.8f;
            go.transform.SetParent(cards_.transform, false);
            go.transform.localPosition = new Vector3(
                    jj,
                    ii,
                    0f);

            go.transform.parent.transform.localScale = new Vector3(scaling, scaling, scaling);

            Shader cardShader = Shader.Find("Shader Graphs/Card");
            int frontTexPropId = Shader.PropertyToID("_FrontTexture");
            string filename = deck[i].Path.Replace(".png", "");
            Texture2D tex = Resources.Load<Texture2D>(filename);
            Renderer rend = go.GetComponentInChildren<Renderer>(); // the one that’s already there
            var mpb = new MaterialPropertyBlock();
            mpb.SetTexture(frontTexPropId, tex);
            mpb.SetFloat(BulbTransparencyID, 0f);
            mpb.SetFloat(LitID, 0f);
            rend.SetPropertyBlock(mpb);
            CardObject tmp = new CardObject(deck[i], go);
            tmp.Data.Index = i;
            createdCards.Add(tmp);
        }

        cards_.transform.position = origin;
        Misc.Randomize(createdCards);
        return createdCards;
    }

   

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
            float onOff = UnityEngine.Random.value < 0.5f ? 1f : 0f;

            foreach (var cardObj in GameState.Instance.cards)
            {
                Renderer rend = cardObj.View.GetComponentInChildren<Renderer>();
                rend.GetPropertyBlock(mpb);
                mpb.SetFloat(LitID, onOff * 0.046f);
                rend.SetPropertyBlock(mpb);
            }

            _bulbMat.SetFloat(OnID, onOff);
            float wait = UnityEngine.Random.Range(0.05f, 0.15f);

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
