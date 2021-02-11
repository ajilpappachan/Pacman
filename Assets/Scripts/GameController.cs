using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SwipeManager))]
public class GameController : MonoBehaviour
{
    [Header("Grid Setup")]
    [SerializeField] GameObject grid;
    [SerializeField] GameObject cellObject;
    [SerializeField] int width = 18;
    [SerializeField] int height = 32;
    Dictionary<GridIndex, GridCell> gridMap = new Dictionary<GridIndex, GridCell>();
    List<GridIndex> activeCells = new List<GridIndex>();

    [Header("UI Setup")]
    [SerializeField] Text percentageText;
    [SerializeField] Text livesText;
    int totalCells;
    int filledCells;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject youWinUI;
    [SerializeField] GameObject youLoseUI;

    [Header("Character Setup")]
    public float BASE_SPEED;
    public int PACMAN_SPEED_MULTIPIER;
    public int SLOWGHOST_SPEED_MULTIPLIER;
    public int FASTGHOST_SPEED_MULTIPLIER;
    SwipeManager swipeManager;

    [Header("Pacman Setup")]
    [SerializeField] GameObject pacmanObject;
    Pacman pacman;
    [SerializeField] int lives = 3;

    [Header("Ghosts Setup")]
    [SerializeField] GameObject slowGhostObject;
    [SerializeField] GameObject fastGhostObject;
    [SerializeField] int slowGhostCount = 3;
    [SerializeField] int fastGhostCount = 2;

    [Header("Power Up Setup")]
    [SerializeField] GameObject powerUpObject;
    [SerializeField] float powerUpSpawnDuration = 5.0f;
    [SerializeField] float powerUpLifeDuration = 8.0f;
    [SerializeField] float powerUpEffectDuration = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        totalCells = (height - 2) * (width - 2);        // Total Cells Excluding the border cells
        filledCells = 0;
        swipeManager = GetComponent<SwipeManager>();    // Initialise Swipe Manager
        gridInit();                                     //Set up Grid
        SpawnPowerUp();                                 //Start PowerUp Timer
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        pacMovement();                                  //Take Input for Player
    }

    #region Grid Management

    //Initialise Grid
    void gridInit()
    {
        //Spawn Cells
        for(int row = 0; row < height; row++)
        {
            for(int column = 0; column < width; column++)
            {
                GridIndex index = new GridIndex(column, row);
                GameObject cell = Instantiate(cellObject, grid.transform.position, Quaternion.identity);
                cell.transform.position = new Vector3(column, 0.0f, row);
                cell.transform.SetParent(grid.transform);
                GridCell cellData = cell.GetComponent<Cell>().cellInit(index, this);
                gridMap.Add(index, cellData);
            }
        }

        //Adjust Camera to Fit the full Grid
        Camera.main.transform.position = new Vector3(width / 2, height, 0.0f);

        //Spawn Borders
        foreach(GridIndex index in gridMap.Keys)
        {
            if(index.x == 0 || index.x == width - 1 || index.y == 0 || index.y == height - 1)
            {
                gridMap[index].setActive(true);
                gridMap[index].setResolved(true);
                continue;
            }
        }
        //Spawn Pacman and Ghosts
        spawnPacman(randomCell());
        for (int c = 0; c < slowGhostCount; c++) spawnSlowGhost(randomCell());
        for (int c = 0; c < fastGhostCount; c++) spawnFastGhost(randomCell());
    }

    //Fill area bound by walls. Derived from Flood Fill Algorithm
    bool floodFill()
    {
        bool didFlood = false;
        foreach (GridIndex index in gridMap.Keys)
        {
            // For every Inactive Object Check if it is bound by walls on at least 2 sides
            if (!gridMap[index].isActive)
            {
                List<GridIndex> activeBounds = new List<GridIndex>();
                List<GridIndex> LeftCells = new List<GridIndex>();
                List<GridIndex> RightCells = new List<GridIndex>();
                List<GridIndex> UpCells = new List<GridIndex>();
                List<GridIndex> DownCells = new List<GridIndex>();
                foreach (GridIndex idx in gridMap.Keys)
                {
                    if (idx.x == index.x)
                    {
                        if (idx.y < index.y)
                        {
                            DownCells.Add(idx);
                        }
                        if (idx.y > index.y)
                        {
                            UpCells.Add(idx);
                        }
                    }
                    if (idx.y == index.y)
                    {
                        if (idx.x < index.x)
                        {
                            LeftCells.Add(idx);
                        }
                        if (idx.x > index.x)
                        {
                            RightCells.Add(idx);
                        }
                    }
                }
                foreach (GridIndex i in LeftCells)
                {
                    if (gridMap[i].isActive && !gridMap[i].isResolved)
                    {
                        activeBounds.Add(i);
                        break;
                    }
                }
                foreach (GridIndex i in RightCells)
                {
                    if (gridMap[i].isActive && !gridMap[i].isResolved)
                    {
                        activeBounds.Add(i);
                        break;
                    }
                }
                foreach (GridIndex i in UpCells)
                {
                    if (gridMap[i].isActive && !gridMap[i].isResolved)
                    {
                        activeBounds.Add(i);
                        break;
                    }
                }
                foreach (GridIndex i in DownCells)
                {
                    if (gridMap[i].isActive && !gridMap[i].isResolved)
                    {
                        activeBounds.Add(i);
                        break;
                    }
                }
                if (activeBounds.Count > 2)
                {
                    gridMap[index].setActive(true);
                    gridMap[index].setResolved(true);
                    filledCells++;
                    activeCells.Add(index);
                    didFlood = true;
                }
            }
        }
        //Resolve all active cells to prevent ghost from touching it
        foreach (GridIndex idx in activeCells)
        {
            gridMap[idx].setResolved(true);
        }
        activeCells.Clear();
        //If player is stuck inside the wall boundary, kill the player
        if (isCaught(pacman.index))
        {
            loseLife();
        }
        return didFlood;
    }

    //Pathfinding using A* Algorithm !!!!Causes Stack Overflow!!!!!

    //public List<GridIndex> findPath(GridIndex start, GridIndex target)
    //{
    //    List<GridIndex> path = new List<GridIndex>();
    //    path = findNext(start, target, path);
    //    return path;
    //}

    //List<GridIndex> findNext(GridIndex index, GridIndex target, List<GridIndex> list)
    //{
    //    List<GridIndex> neighbours = getNeighbourCells(index);
    //    GridIndex bestNeighbour = new GridIndex();
    //    float bestDistance = 15.0f;
    //    foreach(GridIndex neighbour in neighbours)
    //    {
    //        float distance = getVector(index, target).magnitude;
    //        if (distance < bestDistance)
    //        {
    //            bestDistance = distance;
    //            bestNeighbour = neighbour;
    //        }
    //    }
    //    list.Add(bestNeighbour);
    //    list = findNext(bestNeighbour, target, list);
    //    return list;
    //}

    #endregion

    #region Cell Functions
    
    //Check if a cell has wall
    public bool isCellActive(GridIndex index)
    {
        if (gridMap.ContainsKey(index))
            return gridMap[index].isActive;
        else
            Debug.Log(index.x + " " + index.y);
        return false;
    }

    //Add a new wall
    public void setCellActive(GridIndex index)
    {
        gridMap[index].setActive(true);
        filledCells++;
        activeCells.Add(index);
        //If the new wall has at least two active neighbours, check for flood fill
        List<GridIndex> neighbours = getActiveNeighbourCells(index);
        if(neighbours.Count >= 2)
        {
            floodFill();
        }
        //Set the progress UI
        setUIPercentage();
    }

    //check if cell is currently active (i.e, unresolved wall is present)
    public bool isActive(GridIndex index)
    {
        return activeCells.Contains(index);
    }

    //Check if any obect is bound within walls
    public bool isCaught(GridIndex index)
    {
        List<GridIndex> neighbours = getActiveNeighbourCells(index);
        if (neighbours.Count == 4)
            return true;
        else
            return false;
    }

    //Get All neighbouring cells (in 4 directions)
    public List<GridIndex> getNeighbourCells(GridIndex index)
    {
        List<GridIndex> neighbours = new List<GridIndex>();
        if (gridMap.ContainsKey(new GridIndex(index.x + 1, index.y))) neighbours.Add(new GridIndex(index.x + 1, index.y));
        if (gridMap.ContainsKey(new GridIndex(index.x - 1, index.y))) neighbours.Add(new GridIndex(index.x - 1, index.y));
        if (gridMap.ContainsKey(new GridIndex(index.x, index.y + 1))) neighbours.Add(new GridIndex(index.x, index.y + 1));
        if (gridMap.ContainsKey(new GridIndex(index.x, index.y - 1))) neighbours.Add(new GridIndex(index.x, index.y - 1));
        return neighbours;
    }

    //Get All neighbouring cells that has walls
    List<GridIndex> getActiveNeighbourCells(GridIndex index)
    {
        List<GridIndex> neighbours = new List<GridIndex>();
        try
        {
            if (gridMap[new GridIndex(index.x + 1, index.y)].isActive) neighbours.Add(new GridIndex(index.x + 1, index.y));
            if (gridMap[new GridIndex(index.x - 1, index.y)].isActive) neighbours.Add(new GridIndex(index.x - 1, index.y));
            if (gridMap[new GridIndex(index.x, index.y + 1)].isActive) neighbours.Add(new GridIndex(index.x, index.y + 1));
            if (gridMap[new GridIndex(index.x, index.y - 1)].isActive) neighbours.Add(new GridIndex(index.x, index.y - 1));
        }
        catch
        {
            Debug.Log(index.x + " " + index.y);
        }
        return neighbours;
    }

    //Get a random inactive cell from the grid
    GridIndex randomCell()
    {
        GridIndex index = new GridIndex();
        bool spawned = false;
        while (!spawned)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);
            index = new GridIndex(x, y);
            spawned = !gridMap[index].isActive;
        }
        return index;
    }

    //Get a random active cell from the grid
    public GridIndex randomActiveCell()
    {
        if (activeCells.Count > 0)
        {
            int index = Random.Range(0, activeCells.Count);
            return activeCells[index];
        }
        else return new GridIndex();
    }

    //Get the vector distance between two cells
    public Vector2 getVector(GridIndex from, GridIndex to)
    {
        int x = to.x - from.x;
        int y = to.y - from.y;
        return new Vector2(x, y);
    }

    //Get a random boundary wall
    public GridIndex getRandomBoundary()
    {
        List<GridIndex> boundaries = new List<GridIndex>();
        foreach(GridIndex index in gridMap.Keys)
        {
            if (index.x == 0 || index.y == 0 || index.x == (width - 1) || index.y == (height - 1))
                boundaries.Add(index);
        }
        return boundaries[Random.Range(0, boundaries.Count)];
    }

    //Set the resolved status of a cell
    public bool isResolved(GridIndex index)
    {
        return gridMap[index].isResolved;
    }

    #endregion

    #region UI Management
    
    //Modify progress bar UI
    void setUIPercentage()
    {
        int fillPercent = (int)((float)filledCells / (float)totalCells * 100);
        percentageText.text = "Progress: " + fillPercent + "/80";
        if (fillPercent > 80)
            YouWin();
    }

    //Update Lives and UI when player dies and respawn if needed
    public void loseLife()
    {
        lives--;
        livesText.text = "Lives: " + lives;
        pacman.kill();
        foreach (GridIndex index in activeCells)
        {
            gridMap[index].setActive(false);
        }
        activeCells.Clear();
        foreach(GridIndex index in gridMap.Keys)
        {
            if (gridMap[index].isActive)
                gridMap[index].setResolved(true);
        }
        if (lives > 0)
            spawnPacman(randomCell());
        else
            YouLose();

    }

    #endregion

    #region Pacman Management

    //Get input for Player movement
    void pacMovement()
    {
        if (!pacman) return;

        if((Input.GetKey(KeyCode.LeftArrow) || swipeManager.SwipeLeft) && !pacman.isMoving)
        {
            pacman.Move(new Vector2(-1, 0));
            return;
        }
        if ((Input.GetKey(KeyCode.RightArrow) || swipeManager.SwipeRight) && !pacman.isMoving)
        {
            pacman.Move(new Vector2(1, 0));
            return;
        }
        if ((Input.GetKey(KeyCode.UpArrow) || swipeManager.SwipeUp) && !pacman.isMoving)
        {
            pacman.Move(new Vector2(0, 1));
            return;
        }
        if ((Input.GetKey(KeyCode.DownArrow) || swipeManager.SwipeDown) && !pacman.isMoving)
        {
            pacman.Move(new Vector2(0, -1));
            return;
        }
        if(!pacman.isMoving)
        {
            pacman.Move();
            return;
        }
    }

    //spawn Player at any cell
    void spawnPacman(GridIndex index)
    {
        GameObject pacSpawn = gridMap[index].spawnObject(pacmanObject);
        pacman = pacSpawn.GetComponent<Pacman>();
        pacman.pacmanInit(index, this);
    }

    #endregion

    #region Power Up Management

    //Spawn a power up after some time
    public void SpawnPowerUp()
    {
        StartCoroutine("spawnPowerUp");
    }

    IEnumerator spawnPowerUp()
    {
        yield return new WaitForSeconds(powerUpSpawnDuration);
        GridIndex index = randomCell();
        GameObject powerup = gridMap[index].spawnObject(powerUpObject);
        powerup.GetComponent<PowerUp>().powerUpInit(index, this, powerUpLifeDuration);
    }

    //Use powerup to slow down ghosts
    public void UsePowerUp()
    {
        StartCoroutine("usePowerUp");
        spawnPowerUp();
    }

    IEnumerator usePowerUp()
    {
        Ghost[] ghosts = FindObjectsOfType<Ghost>();
        foreach (Ghost ghost in ghosts)
        {
            ghost.slowSpeed(true);
        }
        yield return new WaitForSeconds(powerUpEffectDuration);
        foreach (Ghost ghost in ghosts)
        {
            ghost.slowSpeed(false);
        }
    }

    #endregion

    #region Ghost Management
    
    //Spawn a ghost of fast type
    void spawnFastGhost(GridIndex index)
    {
        GameObject ghostSpawn = gridMap[index].spawnObject(fastGhostObject);
        ghostSpawn.GetComponent<Ghost>().ghostInit(index, this, GhostType.FAST);
    }

    //Spawn a ghost of slow type
    void spawnSlowGhost(GridIndex index)
    {
        GameObject ghostSpawn = gridMap[index].spawnObject(slowGhostObject);
        ghostSpawn.GetComponent<Ghost>().ghostInit(index, this, GhostType.SLOW);
    }

    #endregion

    #region Level Management

    //Win Condition UI
    void YouWin()
    {
        Time.timeScale = 0.0f;
        gameOverUI.SetActive(true);
        youWinUI.SetActive(true);
    }

    //Lose Condition UI
    void YouLose()
    {
        Time.timeScale = 0.0f;
        gameOverUI.SetActive(true);
        youLoseUI.SetActive(true);
    }

    //Restart Game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion
}
