using System.Net.Http.Headers;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Makes the AudioManager a singleton so that it can be called from anywhere without a reference and there is only one instance of it.
    [SerializeField] private AudioSource audioObject;
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
    }

    public void PlaySound(AudioClip audioClip, Transform spawnTransform, float volume) {
        // Spawn in object
        AudioSource audioSource = Instantiate(audioObject, spawnTransform.position, Quaternion.identity);

        // Assign the audioClip
        audioSource.clip = audioClip;
        // Assign volume
        audioSource.volume = volume;
        // Play sound
        audioSource.Play();
        // Get length of clipp
        float clipLength = audioSource.clip.length;
        // Destroy the clip after an amount of time
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSound(AudioClip[] audioClip, Transform spawnTransform, float volume) {

        // Assign a random index
        int rand = Random.Range(0, audioClip.Length);
        // Spawn in object
        AudioSource audioSource = Instantiate(audioObject, spawnTransform.position, Quaternion.identity);

        // Assign the audioClip
        audioSource.clip = audioClip[rand];
        // Assign volume
        audioSource.volume = volume;
        // Play sound
        audioSource.Play();
        // Get length of clipp
        float clipLength = audioSource.clip.length;
        // Destroy the clip after an amount of time
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayClickSound() {
        AudioManager.instance.PlaySound(clickSound, this.transform, 100);
    }
}
