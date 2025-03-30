using Unity.VisualScripting;
using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    [SerializeField] private AudioClip fallDeathSound;
    [SerializeField] private AudioClip burnDeathSound;
    [SerializeField] private AudioClip lavaDeathSound;
    [SerializeField] private AudioClip crushDeathSound;
    private bool dead = false;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Player playerComponent;
    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Trap" && !dead) {
            string trapName = collider.GameObject().name;

            if (trapName == "Lava Kill Box") {
                AudioManager.instance.PlaySound(lavaDeathSound, playerObject.transform, 100);
            }
            else if (trapName == "Drop Kill Box") {
                AudioManager.instance.PlaySound(fallDeathSound, playerObject.transform, 100);
            }
            else if (trapName == "Prefab Kill Box") {
                AudioManager.instance.PlaySound(burnDeathSound, playerObject.transform, 100);
            }
            else if (trapName == "Kill Box") {
                AudioManager.instance.PlaySound(crushDeathSound, playerObject.transform, 100);
            }
            dead = true;

            

            playerComponent.DestroyInput();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneFadeController.instance.LoadScene(0);
        }
    }

    void Update()
    {
        if (transform.position.y < -100 && !dead) {
            dead = true;

            playerComponent.GetComponent<Player>().DestroyInput();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneFadeController.instance.LoadScene(0);
        }
    }
}
