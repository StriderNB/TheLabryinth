using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    private Image image;

    private void Awake()
    {

        image = GetComponent<Image>();
    }

    public IEnumerator FadeInCoroutine(float duration) {
        Color startColor = new Color(image.color.r, image.color.g, image.color.b, 1f);
        Color targetColor = new Color(image.color.r, image.color.g, image.color.b, 0f);

        yield return Fade(startColor, targetColor, duration);
        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration) {
        Color startColor = new Color(image.color.r, image.color.g, image.color.b, 0f);
        Color targetColor = new Color(image.color.r, image.color.g, image.color.b, 1f);

        gameObject.SetActive(true);
        yield return Fade(startColor, targetColor, duration);
    }

    private IEnumerator Fade(Color startColor, Color targetColor, float duration) {
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1) {
            elapsedPercentage = elapsedTime / duration;
            image.color = Color.Lerp(startColor, targetColor, elapsedPercentage);

            yield return null; // Waits until next framme
            elapsedTime += Time.deltaTime;
        }
    }
}
