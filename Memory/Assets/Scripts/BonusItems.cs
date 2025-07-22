using GameNamespace;
using UnityEngine;

public class BonusClickable : MonoBehaviour
{
    public BonusType bonusType;

    void Awake()
    {
        UpdateVisibility();  // hide/show based on default availability
        GameState.Instance.OnBonusAvailabilityChanged += UpdateVisibility;
    }


    void Start()
    {
        
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
        Debug.Log("mouse down");
        GameState.Instance.GrantBonus(bonusType);
    }
}