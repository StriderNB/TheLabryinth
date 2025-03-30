using UnityEngine;

public class DestroyObjectsAbove : MonoBehaviour
{
    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up * 1000, out hit)) {
            Destroy(hit.collider.gameObject);
        }
        if (Physics.Raycast(transform.position, -Vector3.up * 1000, out hit)) {
            Destroy(hit.collider.gameObject);
        }
        if (Physics.Raycast(transform.position, -Vector3.up * 1000, out hit)) {
            Destroy(hit.collider.gameObject);
        }
        if (Physics.Raycast(transform.position, -Vector3.up * 1000, out hit)) {
            Destroy(hit.collider.gameObject);
        }
    }
}
