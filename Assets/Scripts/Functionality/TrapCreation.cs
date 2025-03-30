using UnityEngine;

public class TrapCreation : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects;

    private void Spawn()
    {
        float heightOffset = Random.Range(0.02f, 0.005f);
        int randomObject = Random.Range(0, gameObjects.Length);
        Instantiate(gameObjects[randomObject], new Vector3 (transform.position.x, transform.position.y + 0.1f + heightOffset, transform.position.z), Quaternion.identity);
    }

    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
            Destroy(hit.collider.gameObject);
        }
        Spawn();
        Destroy(this);
    } 
}
