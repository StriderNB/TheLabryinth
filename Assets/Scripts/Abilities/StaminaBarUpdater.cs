using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class StaminaBarUpdater : MonoBehaviour
{
    public static StaminaBarUpdater Instance {
        get => Instance;
    }

    private static StaminaBarUpdater instance;

    private void Start() {
        instance = this;
    }

    [SerializeField] private GameObject imageObject;
    public void Running (bool running) {
        if (running) {
            imageObject.transform.localScale = new Vector3 (0, 1, 1);
        }
        else {
            imageObject.transform.localScale = new Vector3 (1, 1, 1);
        }
        
    }
}
