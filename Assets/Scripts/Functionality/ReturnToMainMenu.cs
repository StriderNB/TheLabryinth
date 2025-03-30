using UnityEngine;

public class ReturnToMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Player player;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            panel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CloseMenu () {
        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMenu () {
        player.DestroyInput();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneFadeController.instance.LoadScene(0);
    }
}
