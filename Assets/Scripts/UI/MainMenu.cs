using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioClip clickSound;
    public void PlayGame () {
        SceneFadeController.instance.LoadScene(1);
    }

    public void QuitGame () {
        Application.Quit();
    }

    public void PlayClickSound () {
        AudioManager.instance.PlaySound(clickSound, this.transform, 100);
    }
}
