using UnityEngine;
using System.Collections;
using TMPro;

public class FadeManager : MonoBehaviour
{
    [Header("Optional: drag refs in Inspector")]
    public Renderer blackScreenRenderer;
    public TMP_Text stageTitleText;

    public float fadeDuration = 0.5f;

    Material blackMat;

    void Awake()
    {
        

        if (blackScreenRenderer)
        {
            blackMat = Instantiate(blackScreenRenderer.material);
            blackScreenRenderer.material = blackMat;
        }
    }


    public void FadeBlackIn() { if (blackMat) StartCoroutine(FadeMatAlpha(1f)); }
    public void FadeBlackOut() { if (blackMat) StartCoroutine(FadeMatAlpha(0f)); }

    IEnumerator FadeMatAlpha(float target)
    {
        Color c = blackMat.color;
        float start = c.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, target, t / fadeDuration);
            blackMat.color = c;
            yield return null;
        }
        c.a = target;
        blackMat.color = c;
    }


    public void ShowStageTitle()
    {
        if (!stageTitleText) return;
        stageTitleText.gameObject.SetActive(true);
        StartCoroutine(FadeTMPAlpha(stageTitleText, 1f));
    }

    public void HideStageTitle()
    {
        if (!stageTitleText) return;
        StartCoroutine(FadeTMPAlpha(stageTitleText, 0f, deactivateOnZero: true));
    }

    IEnumerator FadeTMPAlpha(TMP_Text tmp, float target, bool deactivateOnZero = false)
    {
        Color c = tmp.color;
        float start = c.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, target, t / fadeDuration);
            tmp.color = c;
            yield return null;
        }
        c.a = target;
        tmp.color = c;

        if (deactivateOnZero && Mathf.Approximately(target, 0f))
            tmp.gameObject.SetActive(false);
    }
}
