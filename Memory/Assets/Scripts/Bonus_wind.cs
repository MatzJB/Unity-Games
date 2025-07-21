using UnityEngine;

public class Bonus : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    void OnMouseDown()
    {
        Debug.Log("clicked!");
        if (GameState.Instance != null)
        {
            GameState.Instance.Tornado();
        }
    }

}
