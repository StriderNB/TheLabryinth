using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PurchaseAbilities : MonoBehaviour
{
    public class ability {
        public int Cost1;
        public int Cost2;
        public int Level;
        public string Name;

        public ability (int cost, int cost2, int level, string name)
        {
            Cost1 = cost;
            Cost2 = cost2;
            Level = level;
            Name = name;
        }
    }
    [SerializeField] private DataTransferSO dataTransferSO;
    [SerializeField] private Sprite buyIcon;
    [SerializeField] private Sprite upgradeIcon;
    [SerializeField] private Sprite maxedIcon;
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private List<Image> buyIcons = new List<Image>();
    [SerializeField] private List<ability> abilities = new List<ability>();
    [SerializeField] private List<TMP_Text> text = new List<TMP_Text>();

    void Awake()
    {
        // Create the abilities
        ability sprint = new(150, 300, 0, "Sprint");
        ability rayGun = new(150, 300, 0, "RayGun");
        ability timeBoost = new(150, 300, 0, "Time Boost");
        ability shotgun = new(150, 300, 0, "Shotgun");
        ability placeHolder1 = new(5940, 5940, 0, "PlaceHolder1");
        ability placeHolder2 = new(5940, 5940, 0, "PlaceHolder2");

        // Add them to the list
        abilities.Add(sprint);
        abilities.Add(rayGun);
        abilities.Add(timeBoost);
        abilities.Add(shotgun);
        abilities.Add(placeHolder1);
        abilities.Add(placeHolder2);

        // Update values based on save data

        // Update the shop menu
        UpdateShopFront();
    } 

    void FixedUpdate()
    {
        UpdateShopFront();
    }


    public void BuyAbility (int index) {
        // Check current level, if it is 2 return
        if (abilities[index].Level == 2)
            return;

        int level = abilities[index].Level+1;
        int cost = 0;
        int nextCost = 0;

        // Increase its level, a max of level 2
        Mathf.Clamp(abilities[index].Level = abilities[index].Level + 1, 0, 2);

        // Calculate the cost
        if (level == 1) {
            cost = abilities[index].Cost1;
            nextCost = abilities[index].Cost2;
        }
        else if (level == 2) {
            cost = abilities[index].Cost2;
            nextCost = 0;
        }

        // Check if you can afford it
        if (dataTransferSO.totalTime - cost < 0) {
            Mathf.Clamp(abilities[index].Level = abilities[index].Level - 1, 0, 2);
            Debug.Log("Too poor");
            return;
        }

        // Deduct the moneyyyy
        dataTransferSO.totalTime -= cost;

        // Update the cost text
        if (level == 1) {
            int minutes = Mathf.FloorToInt(nextCost / 60F);
            int seconds = Mathf.FloorToInt(nextCost - minutes * 60);

            string formattedNextCost = string.Format("{0:0}:{1:00}", minutes, seconds);

            text[index].text = formattedNextCost.ToString();
        }
        else if (level == 2) {
            text[index].GameObject().SetActive(false);
        }

        // Remove the shop symbol
        if (level == 1) {
            buyIcons[index].overrideSprite = upgradeIcon;
        }
        else if (level == 2)
        {
            buyIcons[index].overrideSprite = maxedIcon;
        }
    
        // Update the currency text
        int amount = dataTransferSO.totalTime;
        int minutes2 = Mathf.FloorToInt(amount / 60F);
        int seconds2 = Mathf.FloorToInt(amount - minutes2 * 60);
        string formattedCurrencyAmount = string.Format("{0:0}:{1:00}", minutes2, seconds2);

        currencyText.text = formattedCurrencyAmount;

        // Update the SO for transferring data
        dataTransferSO.abilityLevels[index] = level;
    }

    public void UpdateShopFront () {
        // Update the SO for transferring data
        for (var y = 0; y<6; y++) {
            abilities[y].Level = dataTransferSO.abilityLevels[y];
        }
        

        for (var i = 0; i<6; i++) {
            // Remove the shop symbol
            int level = abilities[i].Level;
            if (level == 1) {
                buyIcons[i].overrideSprite = upgradeIcon;
            }
            else if (level == 2) {
                buyIcons[i].overrideSprite = maxedIcon;
            }
            else if (level == 0) {
                buyIcons[i].overrideSprite = buyIcon;
            }

            // Calculate the cost
            int nextCost = 0;
            if (level == 0) {
                nextCost = abilities[i].Cost1;
            }
            else if (level == 1) {
                nextCost = abilities[i].Cost2;
            }

            // Update the cost text
            int minutes = Mathf.FloorToInt(nextCost / 60F);
            int seconds = Mathf.FloorToInt(nextCost - minutes * 60);
            string formattedNextCost = string.Format("{0:0}:{1:00}", minutes, seconds);

            text[i].text = formattedNextCost.ToString();

            if (level == 2) {
                text[i].GameObject().SetActive(false);
            }
            else {
                text[i].GameObject().SetActive(true);
            }
        }

        // Update the currency text
        int amount = dataTransferSO.totalTime;
        int minutes2 = Mathf.FloorToInt(amount / 60F);
        int seconds2 = Mathf.FloorToInt(amount - minutes2 * 60);
        string formattedCurrencyAmount = string.Format("{0:0}:{1:00}", minutes2, seconds2);

        currencyText.text = formattedCurrencyAmount;
    }
} 
