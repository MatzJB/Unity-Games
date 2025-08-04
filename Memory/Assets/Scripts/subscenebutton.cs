using UnityEngine;
using UnityEngine.EventSystems;

public class SubsceneButton : MonoBehaviour
{
    public float scaleFactor = 1.1f;
    public float speed = 10f;

    private Transform target;
    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        target = transform.parent; // the text
        originalScale = target.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        target.localScale = Vector3.Lerp(target.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    void OnMouseEnter()
    {
        targetScale = originalScale * scaleFactor;
    }

    void OnMouseExit()
    {
        targetScale = originalScale;
    }



    void OnMouseDown()
    {
        string subsceneName = ParseTargetName();
        HandleSubsceneClick(subsceneName);
    }


    private string ParseTargetName()
    {
        // Expects name like "Text_GotoPlay"
        string name = target.name;

        if (name.StartsWith("Text_Goto"))
            return name.Substring("Text_Goto".Length);

        return name; // fallback
    }

    private void HandleSubsceneClick(string subsceneName)
    {
        if (subsceneName == "Exit")
        {
            Application.Quit();
            return;
        }

        GameObject subscene = GameObject.Find(subsceneName);
        if (subscene != null)
        {
            Camera.main.transform.position = new Vector3(
                subscene.transform.position.x,
                subscene.transform.position.y,
                Camera.main.transform.position.z
            );
        }
        else
        {
            Debug.LogWarning($"Subscene '{subsceneName}' not found.");
        }
    }
}
