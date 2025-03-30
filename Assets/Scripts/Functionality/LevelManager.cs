using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private DataTransferSO dataTransferSO;
    [SerializeField] private MazeGenerator2 mazeGenerator;
    [SerializeField] private MazeRenderer mazeRenderer;
    [SerializeField] private GameTimer levelTime;
    void Awake()
    {
        int level = dataTransferSO.level;

        if (level == 1) {
            SelectSettings(5,5,0,180);
            return;
        } else if (level == 2) {
            SelectSettings(7,7,3,240);
            return;
        } else if (level == 3) {
            SelectSettings(8,8,20,300);
            return;
        } else if (level == 4) {
            SelectSettings(10,10,10,360);
            return;
        } else if (level == 5) {
            SelectSettings(5,5,200,60);
            return;
        } else if (level == 6) {
            SelectSettings(7,7,250,120);
            return;
        } else if (level == 7) {
            SelectSettings(5,10,250,210);
            return;
        } else if (level == 8) {
            SelectSettings(8,8,250,180);
            return;
        } else if (level == 9) {
            SelectSettings(12,12,400,240);
            return;
        }
    }

    private void SelectSettings (int height, int width, int traps, int time) {
            mazeGenerator.mazeHeight  = height;
            mazeGenerator.mazeWidth  = width;
            mazeRenderer.numberOfTraps = traps;
            levelTime.time = time;
    }
}
