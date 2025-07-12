using UnityEngine;

public class WorldAnimation : MonoBehaviour
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
        float y = Mathf.Abs(Mathf.Cos(t / 60)) * 35;
        float y_max = 40;

        float tt = Mathf.Abs(Mathf.Cos(t / 60 + 2.6f)) * y_max;
        float tumbleWeedRotationSpeed = -.3f + -0.6f * (1 - tt / y_max);

        var pos = transform.localPosition;
        pos.x = t;
        pos.y = y;

        transform.localPosition = pos;

        if (transform.localPosition.x > 1500f) //TODO: hard coded
        {
            transform.localPosition = startPos;
            elapsed = -10f;
        }

        transform.localRotation *= Quaternion.Euler(0, 0, tumbleWeedRotationSpeed);
    }
}
