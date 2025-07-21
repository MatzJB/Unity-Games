using UnityEngine;

public class BonusClickable : MonoBehaviour
{
    public int bonusType;

    void OnEnable()
    {
        GameState.Instance.OnBonusAvailabilityChanged += UpdateVisibility;
        UpdateVisibility();
    }

    void OnDisable()
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