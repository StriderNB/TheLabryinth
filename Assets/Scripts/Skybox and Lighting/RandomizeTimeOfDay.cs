using System.Collections.Generic;
using UnityEngine;

public class RandomizeTimeOfDay : MonoBehaviour
{
    [SerializeField] private List<Material> skyboxMaterials = new List<Material>();
    [SerializeField] private Light sunLight;
    void Start()
    {
        RandomTimeOfDay();
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P)) {
            RandomTimeOfDay();
        }
        #endif
    } 

    private void RandomTimeOfDay () {
        int randomNum = Random.Range(0, skyboxMaterials.Count);

        UpdateSkybox(randomNum);
        UpdateSceneLighting(randomNum);
    }

    private void UpdateSkybox(int index) {
        RenderSettings.skybox = skyboxMaterials[index];
    }

    private void UpdateSceneLighting(int index)
    {
        if (index == 0) 
        {
            RenderSettings.fog = false;
            RenderSettings.fogDensity = 0f;
            sunLight.intensity = 3f;
            RenderSettings.ambientIntensity = 1.2f;
            return;
        } 
        else if (index == 1) 
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.001f;
            sunLight.intensity = 1f;
            RenderSettings.ambientIntensity = 0.9f;
            return;
        } 
        else if (index == 2) 
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.002f;
            sunLight.intensity = 0.15f;
            RenderSettings.ambientIntensity = 0f;
            return;
        } 
        else if (index == 3)
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.001f;
            sunLight.intensity = 0.8f;
            RenderSettings.ambientIntensity = 0.8f;
            return;
        }
        else if (index == 4)
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.002f;
            sunLight.intensity = 0.3f;
            RenderSettings.ambientIntensity = 0.8f;
            return;
        }
        else if (index == 5)
        {
            RenderSettings.fog = false;
            RenderSettings.fogDensity = 0f;
            sunLight.intensity = 3f;
            RenderSettings.ambientIntensity = 1.2f;
            return;
        }
        else if (index == 6)
        {
            RenderSettings.fog = false;
            RenderSettings.fogDensity = 0f;
            sunLight.intensity = 3f;
            RenderSettings.ambientIntensity = 1.5f;
            return;
        }
        else if (index == 7)
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.0025f;
            sunLight.intensity = 0.05f;
            RenderSettings.ambientIntensity = 0f;
            return;
        }
        else if (index == 8)
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.007f;
            sunLight.intensity = 0.2f;
            RenderSettings.ambientIntensity = 0.8f;
            return;
        }
        else if (index == 9)
        {
            RenderSettings.fog = false;
            RenderSettings.fogDensity = 0f;
            sunLight.intensity = 8f;
            RenderSettings.ambientIntensity = 0.7f;
            return;
        }
        else
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.001f;
            sunLight.intensity = 1f;
            RenderSettings.ambientIntensity = 1f;
        }
    } 
}
