using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data Structure to store and modify important details about a cell
public class GridCell
{
    public GridIndex index;
    public bool isActive;
    public bool isResolved;
    public Cell cell;

    //Initialise
    public GridCell(GridIndex index, Cell cell)
    {
        this.index = index;
        this.cell = cell;
        this.isActive = false;
        this.isResolved = false;
    }

    //Set active status and spawn or destroy brick with respect to the same
    public void setActive(bool value)
    {
        if (value) cell.spawnBrick();
        else cell.destroyBrick();
        isActive = value;
    }

    //Spawn any object in the cell
    public GameObject spawnObject(GameObject obj)
    {
        GameObject spawn = cell.spawnObject(obj);
        return spawn;
    }

    //Set resolved status of the cell to prevent ghost from interacting with it
    public void setResolved(bool value)
    {
        isResolved = value;
    }
}
