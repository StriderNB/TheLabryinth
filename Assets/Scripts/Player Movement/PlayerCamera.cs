using UnityEngine;

public struct CameraInput {
    public Vector2 Look;
}
public class PlayerCamera : MonoBehaviour
{

    [SerializeField] private float sensitivity = 0.1f;
    private Vector3 _eulerAngles;
    internal void Initialize(Transform target)
    {
        target.position = target.position;
        transform.rotation = target.rotation;

        transform.eulerAngles = _eulerAngles = target.eulerAngles;
    }

    public void UpdateRotation (CameraInput input) {
        _eulerAngles += new Vector3 (Mathf.Clamp(-input.Look.y, -90, 90), input.Look.x) * sensitivity;
        _eulerAngles = new Vector3 (Mathf.Clamp(_eulerAngles.x, -90, 90), _eulerAngles.y);
        transform.eulerAngles = _eulerAngles;
    }

    public void UpdatePosition (Transform target) {
        transform.position = target.position;
    }
}