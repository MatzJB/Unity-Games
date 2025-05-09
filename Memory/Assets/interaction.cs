using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interaction : MonoBehaviour
{
    Vector3 defaultScale;

    private void Start()
    {
        defaultScale = transform.localScale;
    }

    void OnMouseOver()
    {
        transform.localScale = defaultScale * 1.1f;
    }

    void OnMouseExit()
    {
        transform.localScale = defaultScale;
    }

}
