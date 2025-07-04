using UnityEngine;

public class AbsCosMover : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 90f;
    public float amplitude = 1000f;
    public float jumpAfterSeconds = 5f;

    Vector3 startPos;
    float elapsed;

    void Start()
    {
        startPos = transform.localPosition;
        elapsed = -10;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        float t = elapsed * 200;
        float y = Mathf.Abs(Mathf.Cos(t / 60)) * 40;

        var pos = transform.localPosition;
        pos.x = t;
        pos.y = y;

        transform.localPosition = pos;

        if (transform.localPosition.x > 1000f)
        {
            transform.localPosition = startPos;
            elapsed = -10f;
        }

        transform.localRotation *= Quaternion.Euler(0, 0, -0.5f);


    }
}
