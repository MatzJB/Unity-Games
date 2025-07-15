using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
/*
 Responsible for interactions with the cards. Each card has this script attached to it.
 */

public class Interaction : MonoBehaviour
{
    private Vector3 defaultScale;
    public float hoverScaleFactor = 1.2f;
    public float smoothSpeed = 10f;
    private Vector3 targetScale;
    public GameState gameState;


    private Material _bulbMat;
    //bonuses and penalties are here:
    GameObject bulb;
    Renderer bulbRenderer;

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

    public void Init(GameState gs)
    {
        UnityEngine.Debug.Log($" Interaction initialized gameState! {this.GetHashCode()}");
        gameState = gs;
    }


    void OnMouseDown()
    {
        Debug.Log("Clicked object: " + gameObject.name);

        //for some reason gameState is null here, despite init running for each card... how is that possible?
        CardIndex indexComponent = this.GetComponentInParent<CardIndex>();
        if (indexComponent != null)
        {
            int index = indexComponent.index;
            Debug.Log("Card index: " + index);
            gameState.CardClicked(index);// this does work
        }
    }

    public void FlipCard(bool faceUp)
    {
        Animation anim = this.GetComponentInChildren<Animation>(true);
        if (anim != null)
            anim.Play(faceUp ? "Flip" : "FlipBack");
    }

    public IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public IEnumerator DelayedFlipCard(bool faceUp, float delay)
    {
        yield return new WaitForSeconds(delay);
        FlipCard(faceUp);
    }

    // spin cards that are not done
    public void SpinCards()
    {

    }


    void OnMouseExit()
    {
        targetScale = defaultScale;
    }

}


