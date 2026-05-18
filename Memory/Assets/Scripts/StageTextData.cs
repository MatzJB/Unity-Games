using UnityEngine;
using TMPro;
using System.Collections;

public class StageTextData : MonoBehaviour
{
    public TMP_Text stageLabel; // text shown in the top left corner of the screen
    public TMP_Text titleCard; // text shown between stages


    void OnEnable() { StartCoroutine(Bind()); }
    void OnDisable()
    {
        if (_bound && GameState.Instance != null)
        {
            GameState.Instance.OnStageTextChanged -= OnStageChanged;
            GameState.Instance.OnTitleCardTextChanged -= OnTitleCardChanged;
        }
        _bound = false;
    }

    bool _bound;

    IEnumerator Bind()
    {
        while (GameState.Instance == null) yield return null;

        GameState.Instance.OnStageTextChanged += OnStageChanged;
        GameState.Instance.OnTitleCardTextChanged += OnTitleCardChanged;
        _bound = true;

        // initialize UI from current values
        OnStageChanged(GameState.Instance.StageText);
        OnTitleCardChanged(GameState.Instance.TitleCardText);
    }

    void OnStageChanged(string t) { if (stageLabel) stageLabel.text = t; }
    void OnTitleCardChanged(string t) { if (titleCard) titleCard.text = t; }
}
