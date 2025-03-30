using UnityEngine;

public class RotateSkyboxInMainMenu : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1.3f;

    private void Update () {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
}
