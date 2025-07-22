using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Text))]
public class StageTextDisplay : MonoBehaviour
{

    TMP_Text _label;

    void Awake()
    {
        _label = GetComponent<TMP_Text>();
        if (_label == null)
            Debug.LogError($"[{name}] no TMP_Text component found");
    }


    void OnEnable()
    {
        GameState.Instance.OnStageTextChanged += UpdateLabel;
        _label.text = GameState.Instance.StageText;
    }

    void OnDisable()
    {
        GameState.Instance.OnStageTextChanged -= UpdateLabel;
    }

    void UpdateLabel(string newText)
    {
        _label.text = newText;
    }
}