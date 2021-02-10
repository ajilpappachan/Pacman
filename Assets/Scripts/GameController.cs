using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Grid Setup")]
    [SerializeField] GameObject grid;
    [SerializeField] GameObject cellObject;
    [SerializeField] int width = 18;
    [SerializeField] int height = 32;
    Dictionary<GridIndex, GridCell> gridMap = new Dictionary<GridIndex, GridCell>();
    List<GridIndex> activeCells = new List<GridIndex>();

    [Header("Character Setup")]
    public float BASE_SPEED;

    [Header("Pacman Setup")]
    [SerializeField] GameObject pacmanObject;
    Pacman pacman;

    // Start is called before the first frame update
    void Start()
    {
        gridInit();
    }

    // Update is called once per frame
    void Update()
    {
        pacMovement();
    }

    void gridInit()
    {
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

        Camera.main.transform.position = new Vector3(width / 2, height, 0.0f);

        foreach(GridIndex index in gridMap.Keys)
        {
            if(index.x == 0 || index.x == width - 1 || index.y == 0 || index.y == height - 1)
            {
                gridMap[index].setActive(true);
                gridMap[index].setResolved(true);
                gridMap[index].isBoundary = true;
                continue;
            }

            if(index.x == 1 && index.y == height - 2)
            {
                GameObject pacSpawn = gridMap[index].spawnObject(pacmanObject);
                pacman = pacSpawn.GetComponent<Pacman>();
                pacman.pacmanInit(index, this);
            }
        }
    }

    public bool isCellActive(GridIndex index)
    {
        return gridMap[index].isActive;
    }

    public void setCellActive(GridIndex index)
    {
        gridMap[index].setActive(true);
        activeCells.Add(index);
        List<GridIndex> neighbours = getNeighbourCells(index);
        if(neighbours.Count >= 2)
            floodFill();
    }

    List<GridIndex> getNeighbourCells(GridIndex index)
    {
        List<GridIndex> neighbours = new List<GridIndex>();
        if (gridMap[new GridIndex(index.x + 1, index.y)].isActive)  neighbours.Add(new GridIndex(index.x + 1, index.y));
        if (gridMap[new GridIndex(index.x - 1, index.y)].isActive)  neighbours.Add(new GridIndex(index.x - 1, index.y));
        if (gridMap[new GridIndex(index.x, index.y + 1)].isActive)  neighbours.Add(new GridIndex(index.x, index.y + 1));
        if (gridMap[new GridIndex(index.x, index.y - 1)].isActive)  neighbours.Add(new GridIndex(index.x, index.y - 1));
        return neighbours;
    }

    bool floodFill()
    {
        bool didFlood = false;
        foreach(GridIndex index in gridMap.Keys)
        {
            if(!gridMap[index].isActive)
            {
                List<GridIndex> activeBounds = new List<GridIndex>();
                List<GridIndex> LeftCells = new List<GridIndex>();
                List<GridIndex> RightCells = new List<GridIndex>();
                List<GridIndex> UpCells = new List<GridIndex>();
                List<GridIndex> DownCells = new List<GridIndex>();
                foreach (GridIndex idx in gridMap.Keys)
                {
                    if(idx.x == index.x)
                    {
                        if(idx.y < index.y)
                        {
                            DownCells.Add(idx);
                        }
                        if(idx.y > index.y)
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
                foreach(GridIndex i in LeftCells)
                {
                    if (gridMap[i].isActive && !gridMap[i].isBoundary)
                    {
                        activeBounds.Add(i);
                        break;
                    }
                }
                foreach (GridIndex i in RightCells)
                {
                    if (gridMap[i].isActive && !gridMap[i].isBoundary)
                    {
                        activeBounds.Add(i);
                        break;
                    }
                }
                foreach (GridIndex i in UpCells)
                {
                    if (gridMap[i].isActive && !gridMap[i].isBoundary)
                    {
                        activeBounds.Add(i);
                        break;
                    }
                }
                foreach (GridIndex i in DownCells)
                {
                    if (gridMap[i].isActive && !gridMap[i].isBoundary)
                    {
                        activeBounds.Add(i);
                        break;
                    }
                }
                if (activeBounds.Count > 2)
                {
                    gridMap[index].setActive(true);
                    gridMap[index].setResolved(true);
                    activeCells.Add(index);
                    didFlood = true;
                }
            }
        }
        if(didFlood)
        {
            foreach (GridIndex idx in activeCells)
            {
                gridMap[idx].setResolved(true);
            }
            activeCells.Clear();
        }
        return didFlood;
    }

    void pacMovement()
    {
        if(Input.GetKey(KeyCode.LeftArrow) && !pacman.isMoving)
        {
            pacman.Move(new Vector2(-1, 0));
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow) && !pacman.isMoving)
        {
            pacman.Move(new Vector2(1, 0));
            return;
        }
        if (Input.GetKey(KeyCode.UpArrow) && !pacman.isMoving)
        {
            pacman.Move(new Vector2(0, 1));
            return;
        }
        if (Input.GetKey(KeyCode.DownArrow) && !pacman.isMoving)
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
}
