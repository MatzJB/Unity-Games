using GameNamespace;
using UnityEngine;

public class BonusClickable : MonoBehaviour
{
    public BonusType bonusType;

    void Awake()
    {
        GameState.Instance.OnBonusAvailabilityChanged += UpdateVisibility;
    }

    void Start()
    {
        UpdateVisibility();  // hide/show based on default availability
    }

    void OnDestroy()
    {
        GameState.Instance.OnBonusAvailabilityChanged -= UpdateVisibility;
    }

    void UpdateVisibility()
    {
        bool avail = GameState.Instance.IsBonusAvailable(bonusType);
        gameObject.SetActive(avail);
    }

    void OnMouseDown()
    {
        GameState.Instance.GrantBonus(bonusType);
    }
}