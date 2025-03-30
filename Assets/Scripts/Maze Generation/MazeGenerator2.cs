using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator2 : MonoBehaviour
{
    [Range(5, 100)]
    public int mazeWidth = 5, mazeHeight = 5;  // Dimensions of the maze
    public int startX, startY;                  // The position the algorithm starts from
    MazeCell2[,] maze;

    Vector2Int currentCell;                     // The maze cell we are currently looking at

    List<Direction> directions = new List<Direction>{Direction.Up, Direction.Down, Direction.Left, Direction.Right};   // List of the 4 directions we can go when knocking down walls

    public MazeCell2[,] GetMaze() {
        // Initialize the maze
        maze = new MazeCell2[mazeWidth, mazeHeight];

        // Loop through the maze width and height
        for (int x = 0; x < mazeWidth; x++) {
            for (int y = 0; y < mazeHeight; y++) {
                maze[x, y] = new MazeCell2(x, y);
            }
        }

        CarvePath(startX, startY);
        return maze;
    }

    List<Direction> GetRandomDirections () {
        // Make a copy of the directions list
        List<Direction> dir = new List<Direction>(directions);

        // Make a randomized directions list
        List<Direction> rndDir = new List<Direction>();
        while (dir.Count > 0) {
            int rnd = Random.Range (0, dir.Count);      // Makes a random number at the length of the list
            rndDir.Add(dir[rnd]);                       // Adds the item at random number in the list to the randomized list
            dir.RemoveAt(rnd);                          // Removes the number from the list at index rnd so it cant be chosen again
        }

        return rndDir;
    }

    bool IsCellValid (int x, int y) {
        if (x < 0 ||  y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1 || maze[x, y].visited) {
            return false;
        } else {
            return true;
        }
    }

    Vector2Int CheckNeighbors () {
        //  Gets a list of random directions
        List<Direction> rndDir = GetRandomDirections();

        for (int i = 0; i < rndDir.Count; i++) {
            // Set neighbor coordinates to current cell for now
            Vector2Int neighbor = currentCell;

            // Modify neighbor coordinates based on the random direction
            switch (rndDir[i]) {
                case Direction.Up:
                    neighbor.y++;
                    break;
                case Direction.Down:
                    neighbor.y--;
                    break;
                case Direction.Left:
                    neighbor.x++;
                    break;
                case Direction.Right:
                    neighbor.x--;
                    break;
            }

            // If the neighbor we just tried is valid, we can return that neighbor. If not try again
            if (IsCellValid(neighbor.x, neighbor.y))
                return neighbor;
        }

        // If couldnt find a neighbor in all directions, return the currentcell value
        return currentCell;
    }

    void BreakWalls (Vector2Int primaryCell, Vector2Int secondaryCell) {
        // Only going in one direction at a time so just use if statements

        if (primaryCell.x > secondaryCell.x) {                       // If the primary cell is to the left of the second cell
            maze[primaryCell.x, primaryCell.y].leftWall = false;     // break down the left wall of the primary cell 
        }
        else if (primaryCell.x < secondaryCell.x) {                  // If the primary cell is to the right of the second cell
            maze[secondaryCell.x, secondaryCell.y].leftWall = false; // break down the left wall of the secondary cell 
        }

        if (primaryCell.y < secondaryCell.y) {                       // If the primary cell is below the second cell
            maze[primaryCell.x, primaryCell.y].topWall = false;      // break down the top wall of the primary cell 
        }
        else if (primaryCell.y > secondaryCell.y) {                  // If the primary cell is above the second cell
            maze[secondaryCell.x, secondaryCell.y].topWall = false;  // break down the left wall of the secondary cell 
        }
    }

    // Starting at the x and y ppassed in carves a path through the maze until it encounters a dead end
    void CarvePath (int x, int y) {

        // Makes sure that starting position is within the maze
        if (x < 0 ||  y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1) {
            x = y = 0;
            Debug.LogWarning("Starting position is outside of bounds, defaulting to 0, 0");
        }

        // Set th current cell to the starting positions that were passed in
        currentCell = new Vector2Int(x, y);

        // List to keep track of the current path that has been taken
        List<Vector2Int> path = new List<Vector2Int>();

        // Loop until it hits a dead end
        bool deadEnd = false;
        while (!deadEnd) {
            // Get the next cell were going to try
            Vector2Int nextCell = CheckNeighbors();

            // If that cell has no valid neighbors set deadend to true
            if (nextCell == currentCell) {
                for (int i = path.Count - 1; i>= 0; i--) {
                    currentCell = path[i];          // Set the currentCell to the next step back along the path
                    path.RemoveAt(i);               // Remove i from the path
                    nextCell = CheckNeighbors();    // Check that cell to see if any other neighbors are valid

                    // If a valid cell is found break out of the loop
                    if (nextCell != currentCell)
                        break;
                }

                // If backtracked through the entire path and hasnt found any neighbors the maze has been complete
                if (nextCell == currentCell)
                    deadEnd = true;
            } else {
                BreakWalls(currentCell, nextCell);                  // Set wall flags on these two cells
                maze[currentCell.x, currentCell.y].visited = true;  // Set the cell as visited
                currentCell = nextCell;                             // Move on to the next cell
                path.Add(currentCell);
            }
        }
    }
}

public enum Direction {
    Up,
    Down,
    Left,
    Right
}

public class MazeCell2 {
    public bool visited;
    public int x, y;

    // Each cell only needs two walls to avoid creating 2 walls inside eachother
    public bool topWall;
    public bool leftWall;

    // Return x and y as a Vector2 for convenience
    public Vector2Int position {
        get {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell2 (int x, int y) {

        // The coordinates of this cell in the maze grid
        this.x = x;
        this.y = y;

        // If the algorithm has visited a cell, starts false
        visited = false;

        // All walls are active to start
        topWall = leftWall = true;
    }
}
