using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer instance {get; private set;}
    [Header("Game Objects")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bonusTimerText;
    public DataTransferSO dataTransferSO;
    [Header("Settings")]
    public float time;
    private int bonusTime = 0;
    private bool on = false;
    
    
    private void Awake()
    {
        instance = this;
    }

    public void StartTime()
    {
        on = true;
    }

    void Start()
    {
        if (bonusTime > 0) {
            bonusTimerText.GameObject().SetActive(true);
            //bonusTimerText.GameObject().transform.position = new Vector3();

           // timerText.GameObject().transform.position = new Vector3();
        }
        else if (bonusTime == 0){
            bonusTimerText.GameObject().SetActive(false);
            //timerText.GameObject().transform.position = new Vector3();
        }
    }

    private void Update()
    {
        if (time <= 1) {
            on = false;
            EndTime(); 

            GameObject.Find("Player").GetComponent<Player>().DestroyInput();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneFadeController.instance.LoadScene(0);           
        }


        if (!on)
            return;

        time -= Time.deltaTime;

        if (bonusTime != 0) {
            if (time <= bonusTime) {
                bonusTimerText.text = "+ 0:" + Mathf.FloorToInt(time);
            }
            else {
                if (bonusTime == 30) {
                    float minutes = Mathf.FloorToInt((time - 30) / 60);
                    float seconds = Mathf.FloorToInt((time - 30) % 60);
                    timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
                    return;
                }
                else if (bonusTime == 15) {
                    float minutes = Mathf.FloorToInt((time - 15) / 60);
                    float seconds = Mathf.FloorToInt((time - 15) % 60);
                    timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
                }
            }            
        }
        else {
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }

        
    }

    public void EndTime () {
        if (on == true) {
            on = false;
            int finalTime = Mathf.RoundToInt(time);

            dataTransferSO.totalTime = dataTransferSO.totalTime + finalTime;
        }
    }

    public void BonusTimer(int timeIncrease) {
        bonusTime = timeIncrease;
        time += timeIncrease;

        bonusTimerText.text = "+ 0:" + bonusTime;
    }
}
