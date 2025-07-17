using Assets;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Analytics;

// TODO: refactor this file, move content from createCards to Interaction.cs, Card.cs,...
// Question: what happens if I switch stage, can I do that in real time in the editor?

public class CreateCards : MonoBehaviour
{
    float startTime;
    GameObject cloud;
    Bounds cardBounds; // defined by the "cloud" gameObject
    GameObject lightBulb;
    public GameState gameState;
    GameObject cards__;
    //private int OnID = Shader.PropertyToID("_on");
    //private int LitID = Shader.PropertyToID("_lit");
    int OnID = Shader.PropertyToID("_on"); //bulb
    int LitID = Shader.PropertyToID("_lit"); //card
    int BulbTransparencyID = Shader.PropertyToID("_transparency");


    void Start()
    {
        gameState = new GameState();
        UnityEngine.Debug.Log("gamestate " + gameState);
        UnityEngine.Debug.Assert(gameState != null, "Failed to add GameState component");
        UnityEngine.Debug.Log("Creating GameState object");

        cloud = GameObject.Find("Cloud");
        cardBounds = Misc.GetBounds("Cloud");
        lightBulb = GameObject.Find("Light bulb");

        InitCards();
        cards__ = GameObject.Find("master_card"); //master_card or Cards

        startTime = Time.realtimeSinceStartup;
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


        float width = cardBounds.max.x - cardBounds.min.x;
        float height = cardBounds.max.y - cardBounds.min.y;

        if (width == 0)
        {
            UnityEngine.Debug.LogError("Card width is zero. Check the prefab’s renderer.");
            return;
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

            Shader cardShader = Shader.Find("Shader Graphs/Card");
            int frontTexPropId = Shader.PropertyToID("_FrontTexture");
            string filename = gameState.deck[i].Path.Replace(".png", "");
            Texture2D tex = Resources.Load<Texture2D>(filename);
            Renderer rend = go.GetComponentInChildren<Renderer>(); // the one that’s already there
            var mpb = new MaterialPropertyBlock();
            mpb.SetTexture(frontTexPropId, tex);
            mpb.SetFloat(BulbTransparencyID, 0f);
            mpb.SetFloat(LitID, 0f);
            rend.SetPropertyBlock(mpb);
            CardObject tmp = new CardObject(gameState.deck[i], go);
            tmp.Data.Index = i;
            // add cards using index because otherwise we cannot randomize them
            gameState.AddCard(tmp, i);
        }

        cards_.transform.position = origin;
        Misc.Randomize(gameState.cards);
    }

    // Find the board and places cards randomly
    public void InitCards() // load level
    {
        LevelState levelData = gameState.levelStates[gameState.stage];

        BuildBoard();
        gameState.StartCurrentStage();
    }
    // Update is called once per frame


    public void TriggerLamp()
    {
        StartCoroutine(FlickerThenFade());
    }

    //would be nicer if this function is located in another persistant class so we dont have to fetch the bulb every time
    //TODO: new to fix
    private IEnumerator FlickerThenFade()
    {
        Material _bulbMat = lightBulb.GetComponent<Renderer>().material;
        Material card_mat = cards__.GetComponent<Renderer>().material;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        float elapsed = 0f;
        Vector3 lightBulbPosition = lightBulb.transform.localPosition;



        for (float t = 0f; t < 1f; t += Time.deltaTime)
        {
            float v = Mathf.Lerp(0f, 1f, t);

            mpb = lightBulb.GetComponentInChildren<Renderer>().GetPropertyBlock(mpb);
            _bulbMat.SetFloat(BulbTransparencyID, v);
            lightBulb.transform.position = lightBulb.transform.position + new Vector3(0, 0.5f, 0);
        }

        //TODO: check that the cards are updated properly
        while (elapsed < 3f)
        {
            float onOff = UnityEngine.Random.value < 0.5f ? 1f : 0f;

            foreach (var cardObj in gameState.cards)
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

            foreach (var cardObj in gameState.cards)
            {
                Renderer rend = cardObj.View.GetComponentInChildren<Renderer>();
                rend.GetPropertyBlock(mpb);

                mpb.SetFloat(LitID, 0f);
                rend.SetPropertyBlock(mpb);

            }




            ////card_mat.SetFloat(LitID, 1f);
            //foreach (var cardObj in gameState.cards)
            //{
            //    Renderer rend = cardObj.View.GetComponentInChildren<Renderer>();
            //    rend.GetPropertyBlock(mpb);

            //    mpb.SetFloat(LitID, 1f);
            //    rend.SetPropertyBlock(mpb);

            //    _bulbMat.SetFloat(OnID, onOff);
            //}
            ////new
            //foreach (var cardObj in gameState.cards)
            //{
            //    Renderer rend = cardObj.View.GetComponentInChildren<Renderer>();
            //    rend.GetPropertyBlock(mpb);

            //    for (float t = 0f; t < 1f; t += Time.deltaTime)
            //    {
            //        float v = Mathf.Lerp(1f, 0f, t);
            //        _bulbMat.SetFloat(OnID, v);
            //        mpb.SetFloat(LitID, v);
            //        rend.SetPropertyBlock(mpb);

            //        yield return null;
            //    }
            //}

            //foreach (var cardObj in gameState.cards)
            //{
            //    Renderer rend = cardObj.View.GetComponentInChildren<Renderer>();
            //    rend.GetPropertyBlock(mpb);
            //    for (float t = 0f; t < 1f; t += Time.deltaTime)
            //    {
            //        float v = Mathf.Lerp(1f, 0f, t);

            //        _bulbMat.SetFloat(OnID, v);
            //        mpb.SetFloat(LitID, v);
            //        rend.SetPropertyBlock(mpb);
            //        yield return null;
            //    }
            //}

            //_bulbMat.SetFloat(OnID, 0f);
            //foreach (var cardObj in gameState.cards)
            //{
            //    Renderer rend = cardObj.View.GetComponentInChildren<Renderer>();
            //    rend.GetPropertyBlock(mpb);
            //    mpb.SetFloat(LitID, 0f);
            //    rend.SetPropertyBlock(mpb);
            //}
            //_bulbMat.SetFloat(OnID, 0f);


        }

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
    }
}



