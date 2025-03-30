using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeController : MonoBehaviour
{
    public static SceneFadeController instance;
    [SerializeField] private float fadeDuration = 1.5f;
    private SceneFade scenefade;

    private void Awake()
    {
        instance = this;

        scenefade = GetComponentInChildren<SceneFade>();
    }
    private IEnumerator Start () {
        yield return scenefade.FadeInCoroutine(fadeDuration);
    }

    public void LoadScene(int sceneIndex) {
        StartCoroutine(LoadScenes(sceneIndex));
    }

    private IEnumerator LoadScenes (int sceneIndex) {
        yield return scenefade.FadeOutCoroutine(fadeDuration);
        yield return SceneManager.LoadSceneAsync(sceneIndex);
    }
}
