using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class TitleCardMini : MonoBehaviour
{
    public Renderer blackScreen;
    public TMP_Text title;
    public float duration = 0.5f;
    Material m;

    void Awake()
    {
        if (blackScreen) { m = Instantiate(blackScreen.material); blackScreen.material = m; SetA(m, 0); }
        if (title) { SetA(title, 0); title.gameObject.SetActive(false); }
    }

    public void Run(string titleText, Action work)
    {
        StopAllCoroutines();
        StartCoroutine(Flow(titleText, work));
    }

    IEnumerator Flow(string t, Action work)
    {
        yield return Fade(() => GetA(m), a => SetA(m, a), 1);
        if (title) { title.text = t; title.gameObject.SetActive(true); yield return Fade(() => GetA(title), a => SetA(title, a), 1); }
        work?.Invoke();
        if (title) { yield return Fade(() => GetA(title), a => SetA(title, a), 0); title.gameObject.SetActive(false); }
        yield return Fade(() => GetA(m), a => SetA(m, a), 0);
    }

    IEnumerator Fade(Func<float> get, Action<float> set, float target)
    {
        float start = get(), t = 0f;
        while (t < duration) { t += Time.deltaTime; set(Mathf.Lerp(start, target, t / duration)); yield return null; }
        set(target);
    }

    static float GetA(Material mat) => mat ? mat.color.a : 0f;
    static void SetA(Material mat, float a) { if (!mat) return; var c = mat.color; c.a = a; mat.color = c; }
    static float GetA(TMP_Text tmp) => tmp ? tmp.color.a : 0f;
    static void SetA(TMP_Text tmp, float a) { if (!tmp) return; var c = tmp.color; c.a = a; tmp.color = c; }
}
