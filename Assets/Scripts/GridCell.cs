using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public GridIndex index;
    public bool isActive;
    public bool isResolved;
    public Cell cell;
    public bool isBoundary;

    public GridCell(GridIndex index, Cell cell)
    {
        this.index = index;
        this.cell = cell;
        this.isActive = false;
        this.isResolved = false;
        this.isBoundary = false;
    }

    public void setActive(bool value)
    {
        if (value) cell.spawnBrick();
        else cell.destroyBrick();
        isActive = value;
    }


    public GameObject spawnObject(GameObject obj)
    {
        GameObject spawn = cell.spawnObject(obj);
        return spawn;
    }

    public void setResolved(bool value)
    {
        isResolved = value;
    }
}
