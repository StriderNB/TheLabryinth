using UnityEngine;

public class PlayerExit : MonoBehaviour
{
    [SerializeField] private Player player;
    private float elapsedTime;
    private Vector3 spawnPosition;
    private bool found = false;
    private GameObject platform;

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == 4) {
            found = true;
            platform = collider.gameObject;
            spawnPosition = platform.transform.position;
        }
    }

    void Update()
    {
        if (found) {
            elapsedTime += Time.deltaTime;
            float percentComplete = elapsedTime / 15f;
            platform.transform.position = Vector3.Lerp(spawnPosition, new Vector3(spawnPosition.x, spawnPosition.y - 100f, spawnPosition.z), Mathf.SmoothStep(0, 1, percentComplete));

            if (transform.position.y <= -10f) {
                GameTimer.instance.EndTime();

                player.DestroyInput();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                SceneFadeController.instance.LoadScene(0);
            }
        }
    }
}
