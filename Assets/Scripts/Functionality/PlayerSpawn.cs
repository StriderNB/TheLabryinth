using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private Vector3 spawnPosition;
    private float elapsedTime = 0;
    private GameObject playerObject = null;
    bool playerMoved = false;
    private float percentCompleted;
    public AudioClip elevatorSound;
    public AudioClip alarmSound;
    void Start()
    {
        spawnPosition = transform.position;

        GameTimer.instance.StartTime();

        AudioManager.instance.PlaySound(elevatorSound, transform, 100);
        AudioManager.instance.PlaySound(alarmSound, transform, 100);
    }

    void Update()
    {
        if (playerObject == null) {
            playerObject = GameObject.Find("Player");
        }

        if (playerObject != null & !playerMoved) {
            playerMoved = true;
            playerObject.GetComponentInChildren<PlayerCharacter>().SetPosition(spawnPosition, true);

            GameTimer.instance.StartTime();
        }
        if (percentCompleted != 1) {
            playerObject.GetComponentInChildren<PlayerCharacter>().LerpPosition(spawnPosition, new Vector3(spawnPosition.x, spawnPosition.y + 27.5f, spawnPosition.z), 4.5f, true);
        }

        elapsedTime += Time.deltaTime;
        float percentComplete = elapsedTime / 5f;
        transform.position = Vector3.Lerp(spawnPosition, new Vector3(spawnPosition.x, spawnPosition.y + 27.5f, spawnPosition.z), Mathf.SmoothStep(0, 1, percentComplete));

        if (Mathf.FloorToInt(percentComplete) == 1f) {
            this.enabled = false;
        }
    } 
}
