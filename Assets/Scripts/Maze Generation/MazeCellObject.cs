using UnityEngine;

public class MazeCellObject : MonoBehaviour
{
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject bottomWall;
    [SerializeField] private GameObject topWall;
    [SerializeField] private GameObject bottomFloor;

    public void Init (bool top, bool bottom, bool right, bool left, bool floor) {
        topWall.SetActive(top);
        bottomWall.SetActive(bottom);
        leftWall.SetActive(left);
        rightWall.SetActive(right);
        bottomFloor.SetActive(floor);
    }
}
