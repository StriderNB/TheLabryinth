using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] MazeGenerator2 mazeGenerator2;
    [Space]
    [SerializeField] GameObject MazeCellPrefab;
    [SerializeField] GameObject trapPrefab;
    [SerializeField] GameObject exitPrefab;
    [SerializeField] GameObject entrancePrefab;
    [Space]
    public int numberOfTraps = 0;
    [Space]

    // This is the physical size of the maze cells, getting this wrong will cause overllaping walls or gaps between cells
    [SerializeField] private float cellSize = 1f;

    public void Start() {
        CreateMaze();
    }

    public void CreateMaze () {
        // Get a maze from the maze generator script
        MazeCell2[,] maze = mazeGenerator2.GetMaze();

        // Loop through every cell in the maze
        for (int x = 0; x < mazeGenerator2.mazeWidth; x++) {
            for (int y = 0; y < mazeGenerator2.mazeHeight; y++) {
                // Instantiate a new maze cell prefab as a child of the MazeRenderer object
                GameObject newCell = Instantiate(MazeCellPrefab, new Vector3((float)x * cellSize, 0f, (float)y * cellSize), Quaternion.identity, transform);

                // Offset the wall positions of the cell prefab
                OffsetWallPositions(newCell);

                // Get a reference to the cells MazeCellPrefab script
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                // Determin which walls need to be active
                bool top = maze[x, y].topWall;
                bool left = maze[x, y].leftWall;
                bool floor = true;

                // Bottom and right walls are inactive by default unless we are at the bottom or right edge of the maze
                bool right = false;
                bool bottom = false;
                if (x == mazeGenerator2.mazeWidth - 1) right = true;
                if (y == 0) bottom = true;

                // Generate enterance
                if (x == Mathf.Round(mazeGenerator2.mazeWidth / 2) && y == Mathf.Round(mazeGenerator2.mazeHeight / 2)) {
                    floor = false;
                    Instantiate(entrancePrefab, new Vector3((float)x * cellSize, 0f, y * cellSize), Quaternion.identity, transform);
                }

                // Remove inactive walls tags so that the wall destroy script works
                if (!top) {
                    newCell.transform.GetChild(2).GameObject().layer = 0;
                }
                if (!bottom) {
                    newCell.transform.GetChild(3).GameObject().layer = 0;
                }
                if (!right) {
                    newCell.transform.GetChild(1).GameObject().layer = 0;
                }
                if (!left) {
                    newCell.transform.GetChild(0).GameObject().layer = 0;
                }

                // Set the layer of the edge walls and remove the slide script
                if (x == 0) { // Working left
                    GameObject child = newCell.transform.GetChild(0).GameObject();
                    child.layer = 8;
                    child.GetComponent<slide>().enabled = false;

                    child.transform.GetChild(0).GameObject().layer = 8;
                }
                if (y == 0) {
                    GameObject child = newCell.transform.GetChild(3).GameObject();

                    newCell.transform.GetChild(3).GameObject().layer = 8;
                    newCell.transform.GetChild(3).GetComponent<slide>().enabled = false;

                    child.transform.GetChild(0).GameObject().layer = 8;
                }
                if (x == mazeGenerator2.mazeWidth - 1) {
                    GameObject child = newCell.transform.GetChild(1).GameObject();

                    newCell.transform.GetChild(1).GameObject().layer = 8;
                    newCell.transform.GetChild(1).GetComponent<slide>().enabled = false;

                    child.transform.GetChild(0).GameObject().layer = 8;
                }
                if (y == mazeGenerator2.mazeHeight - 1) {
                    GameObject child = newCell.transform.GetChild(2).GameObject();

                    newCell.transform.GetChild(2).GameObject().layer = 8;
                    newCell.transform.GetChild(2).GetComponent<slide>().enabled = false;

                    child.transform.GetChild(0).GameObject().layer = 8;
                }
 
                mazeCell.Init(top, bottom, right, left, floor);
            }
        }

        GenerateExit();
    }

    private void GenerateTraps (Vector2 exit) {
         List<Vector2> position = new List<Vector2>();
         bool used = false;
        
        // List of used nums so there are no duplicates
        for (int count=0; count<numberOfTraps; count++) {
            used = false;

            int RandomXNum = Random.Range(0, mazeGenerator2.mazeWidth);
            int RandomYNum = Random.Range(0, mazeGenerator2.mazeHeight);

            // Loops through list of spawned traps and if the new index is equal to an old one it wont spawn
            for (int x = 0; x < position.Count; x++) {
                if (new Vector2(RandomXNum, RandomYNum) == position[x] || new Vector2(RandomXNum, RandomYNum) == exit) {
                    used = true;
                }
            }

            // Spawns if the index is unused, and not in the center, then adds the inded to the used list
            if (RandomXNum != Mathf.Round(mazeGenerator2.mazeWidth / 2) && RandomYNum != Mathf.Round(mazeGenerator2.mazeHeight / 2) && used == false) {
                position.Add(new Vector2 (RandomXNum, RandomYNum));
                Instantiate(trapPrefab, new Vector3((float)RandomXNum * cellSize, 1f, (float)RandomYNum * cellSize), Quaternion.identity, transform);                
            }


        }
    }

    private void GenerateExit() {
        // So that the exit generates on the edge of the maze
        int RandomXNum1 = (Random.Range(0, 2) == 1) ? Random.Range(0, 2) : Random.Range(mazeGenerator2.mazeWidth - 2, mazeGenerator2.mazeWidth);

        int RandomXNum2 = (Random.Range(0, 2) == 1) ? Random.Range(0, 2) : Random.Range(mazeGenerator2.mazeHeight - 2, mazeGenerator2.mazeHeight);

        Instantiate(exitPrefab, new Vector3((float)RandomXNum1 * cellSize, 0f, RandomXNum2 * cellSize), Quaternion.identity, transform);

        GenerateTraps(new Vector2(RandomXNum1, RandomXNum2));
    }

    // Really bad code for offsetting the walls position to prevent z fighting
    private void OffsetWallPositions(GameObject cell) {
        GameObject leftWall = cell.transform.GetChild(0).gameObject;
        GameObject rightWall = cell.transform.GetChild(1).gameObject;
        GameObject bottomWall = cell.transform.GetChild(2).gameObject;
        GameObject topWall = cell.transform.GetChild(3).gameObject;

        var tempLeft = leftWall.transform.position;
        tempLeft.x = tempLeft.x + Random.Range(Random.Range(0.01f, 0.001f), Random.Range(-0.01f, -0.001f));
        leftWall.transform.position = tempLeft;

        var tempRight = rightWall.transform.position;
        tempRight.x = tempRight.x + Random.Range(Random.Range(0.01f, 0.001f), Random.Range(-0.01f, -0.001f));
        rightWall.transform.position = tempRight;

        var tempBottom = bottomWall.transform.position;
        tempBottom.z = tempBottom.z + Random.Range(Random.Range(0.01f, 0.001f), Random.Range(-0.01f, -0.001f));
        bottomWall.transform.position = tempBottom;

        var tempTop = topWall.transform.position;
        tempTop.z = tempTop.z + Random.Range(Random.Range(0.01f, 0.001f), Random.Range(-0.01f, -0.001f));
        topWall.transform.position = tempTop;
    }
}
