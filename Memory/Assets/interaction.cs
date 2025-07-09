using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private Vector3 defaultScale;
    public float hoverScaleFactor = 1.2f;
    public float smoothSpeed = 10f;

    private Vector3 targetScale;

    void Start()
    {
        defaultScale = transform.localScale;
        targetScale = defaultScale;
    }

    
    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            smoothSpeed * Time.deltaTime
        );
    }

    void OnMouseEnter()
    {
        targetScale = defaultScale * hoverScaleFactor;
    }

    // TODO: use this instead of ray tracing in createCards
    void OnMouseUpAsButton()
    {
        //Debug.Log("I hit a card!");

    }

    void OnMouseExit()
    {
        targetScale = defaultScale;
    }

}

