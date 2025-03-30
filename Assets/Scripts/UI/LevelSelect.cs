using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSelect : MonoBehaviour, IPointerClickHandler
{
    public DataTransferSO dataTransferSO;
    public int level;
    [SerializeField] private Texture selectTexture;
    [SerializeField] private Texture normalTexture;
    [SerializeField] private MainMenu mainMenu;
    public void OnPointerClick(PointerEventData eventData)
    {
        dataTransferSO.level = level;
        this.GameObject().GetComponent<Renderer>().material.SetTexture("_BaseMap", selectTexture);
        mainMenu.PlayClickSound();
    }

    void FixedUpdate () {
        if (dataTransferSO.level != level) {
            this.GameObject().GetComponent<Renderer>().material.SetTexture("_BaseMap", normalTexture);
        }
        if (dataTransferSO.level == level) {
            this.GameObject().GetComponent<Renderer>().material.SetTexture("_BaseMap", selectTexture);
        }
    }
}
