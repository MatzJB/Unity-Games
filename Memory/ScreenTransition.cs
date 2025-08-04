public class ScreenTransition : MonoBehaviour
{
    public MeshRenderer blackScreen;
    public Transform cameraRig;
    public Transform locationPlay;
    public Camera mainCamera;
    private Dictionary<string, Transform> subsceneAnchors;

    void Awake()
    {
        var anchorRoot = GameObject.Find("SubsceneAnchors");
        subsceneAnchors = anchorRoot.GetComponentsInChildren<Transform>()
            .Where(t => t != anchorRoot.transform)
            .ToDictionary(t => t.name, t => t);
    }


    private void Start()
    {
        cameraRig = Camera.main.transform;

        StartCoroutine(TransitionToPlayScene());
    }


    public IEnumerator TransitionToPlayScene()
    {
        yield return FadeBlackScreen(1f);

        // move camera
        locationPlay = GameObject.Find("LocationPlay").transform;
        cameraRig.position = locationPlay.position;

        // fade in some scene text etc...
        yield return new WaitForSeconds(1f);

        yield return FadeBlackScreen(0f);
    }

    private IEnumerator FadeBlackScreen(float targetAlpha)
    {
        var mat = blackScreen.material;
        Color c = mat.color;
        float startAlpha = c.a;
        float duration = 1f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            mat.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        mat.color = c;
    }
}
