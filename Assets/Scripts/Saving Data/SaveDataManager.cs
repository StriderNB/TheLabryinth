using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField] private DataTransferSO dataTransferSO;
    [SerializeField] private PurchaseAbilities purchaseAbilities;
    private void Awake()
    {
        SaveSystem.init();
    }
    void Start()
    {
        //Load();
    } 
    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F)) {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            Load();
        }
        #endif
    }

    public void Save() {
        SaveObject saveObject = new SaveObject {          
            time = dataTransferSO.totalTime,
            level = 0,
            activeAbilities = dataTransferSO.abilityLevels,
        };

        string json = JsonUtility.ToJson(saveObject);
        SaveSystem.Save(json);
        Debug.Log("saved: " + json);
    }

    public void Load() {
        string saveString = SaveSystem.Load();
        if (saveString != null) {
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

            // set time currency
            // set the active level
            // set the active abilities
            Debug.Log("loaded save");

            // Save data to SO
            dataTransferSO.totalTime = saveObject.time;
            dataTransferSO.abilityLevels = saveObject.activeAbilities;

            purchaseAbilities.UpdateShopFront();
        }
    }

    private class SaveObject {
        public int time;
        public int level;
        public List<int> activeAbilities;
    }
}
