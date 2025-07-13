using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Responsible for interactions with the cards. Each card has this script attached to it.
 */

public class Interaction : MonoBehaviour
{
    private Vector3 defaultScale;
    public float hoverScaleFactor = 1.2f;
    public float smoothSpeed = 10f;
    private Vector3 targetScale;
    GameState gs;


    void Start()
    {
        defaultScale = transform.localScale;
        targetScale = defaultScale;
    }
    
    void Update()
    {
        // Smoothly scale the card to the target scale when hovering over the card
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

    public void Init(GameState manager)
    {
        gs = manager;
    }

    void OnMouseDown()
    {
        gs.CardClicked(this);   // send the whole card object
    }



    void OnMouseExit()
    {
        targetScale = defaultScale;
    }

}

