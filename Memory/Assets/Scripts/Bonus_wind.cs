using UnityEngine;

public class Bonus_wind : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    void OnMouseDown()
    {
        Debug.Log("clicked!");
        GameState.Instance.Tornado();

    }

}
