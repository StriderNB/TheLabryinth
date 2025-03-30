using UnityEngine;

public class TimerAbility : MonoBehaviour
{
    [SerializeField] private DataTransferSO dataTransferSO;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private int level1TimeIncrease = 15;
    [SerializeField] private int level2TimeIncrease = 30;
    void Awake()
    {
        int level = dataTransferSO.abilityLevels[3]; 
        int extraTime = 0;

        if (level == 0) {
            return;
        } else if (level == 1) {
            extraTime = level1TimeIncrease;
            gameTimer.BonusTimer(extraTime);
        } else if (level == 2) {
            extraTime = level2TimeIncrease;
            gameTimer.BonusTimer(extraTime);
        }
    }
}
