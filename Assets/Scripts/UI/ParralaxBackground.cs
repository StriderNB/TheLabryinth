using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    [SerializeField] private float smoothTime = .3f;
    [SerializeField] private float moveMultiplier = -20;
    private Vector3 velocity;
    private Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        transform.position = Vector3.SmoothDamp(transform.position, startPos + (offset * moveMultiplier), ref velocity, smoothTime);
    }
}
