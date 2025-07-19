using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
/*
 Responsible for interactions and animation of each card. 
 Each card has this script attached to it.

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
    Time startTime;
    float randomOffset;//used to get unique rotations for animations and placement of cards

    void Start()
    {
        defaultScale = transform.localScale;
        targetScale = defaultScale;
        randomOffset = UnityEngine.Random.value;
    }

    void Update()
    {
        // Smoothly scale the card to the target scale when hovering over the card
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
        smoothSpeed * Time.deltaTime
        );

        if (gameState != null)
        {
            float elapsedSeconds = Time.realtimeSinceStartup - gameState.startTime;
            float y = Mathf.Cos(elapsedSeconds / 60) * 30;
            float elapsedMs = elapsedSeconds * 1000f;

            transform.parent.position = new Vector3(transform.parent.position.x,
                                                  transform.parent.position.y + 0.02f * Mathf.Cos(elapsedSeconds + transform.parent.position.x % 5 + transform.parent.position.y),
                                                    transform.parent.position.z);

            float angle = Mathf.Cos(transform.parent.position.y + 2f * elapsedSeconds) * 5f;
            transform.parent.localEulerAngles = new Vector3(0, 0, angle);
        }
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
        CardIndex indexComponent = this.GetComponentInParent<CardIndex>();
        if (indexComponent != null)
        {
            int index = indexComponent.index;
            gameState.CardClicked(index);
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


 public IEnumerator SpinCard(float duration, int revolutions)
    {
        // apply randomOffset but ensure at least one revolution
        revolutions = Mathf.Max(1, Mathf.RoundToInt(revolutions * randomOffset));

        float totalDegrees = 360f * revolutions;
        Quaternion startRot = transform.localRotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float angle = Mathf.Lerp(0f, totalDegrees, t);
            transform.localRotation = startRot * Quaternion.Euler(0f, angle, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = startRot * Quaternion.Euler(0f, totalDegrees, 0f);
    }



    public IEnumerator DelayedFlipCard(bool faceUp, float delay)
    {
        yield return new WaitForSeconds(delay);
        FlipCard(faceUp);
    }




    void OnMouseExit()
    {
        targetScale = defaultScale;
    }

}


