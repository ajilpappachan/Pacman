using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    GridIndex index;
    GridCell cell;
    GameController controller;
    [SerializeField] GameObject brickObject;
    [SerializeField] GameObject spawner;
    GameObject spawnedBrick;
    GameObject occupiedObject;

    //Initialise Cell
    public GridCell cellInit(GridIndex index, GameController controller)
    {
        this.index = index;
        this.controller = controller;
        cell = new GridCell(index, this);
        return cell;
    }

    //Spawn a brick object
    public void spawnBrick()
    {
        if (cell.isActive) return;
        spawnedBrick = Instantiate(brickObject, spawner.transform.position, Quaternion.identity);
        spawnedBrick.transform.SetParent(spawner.transform);
    }

    //Destroy brick object
    public void destroyBrick()
    {
        if (!cell.isActive) return;
        Destroy(spawnedBrick);
    }

    //Spawn any object if no brick present
    public GameObject spawnObject(GameObject obj)
    {
        if (cell.isActive) { Debug.Log("IS ACTIVE"); return null; }
        occupiedObject = Instantiate(obj, spawner.transform.position, Quaternion.identity);
        return occupiedObject;
    }

    //Check if Player left the cell to spawn a brick
    private void OnTriggerExit(Collider other)
    {
        Pacman pacman;
        if(other.TryGetComponent(out pacman))
        {
            controller.setCellActive(index);
            if(occupiedObject)
                occupiedObject = null;
        }
        //For any other object other than player
        if(other.gameObject == occupiedObject)
        {
            occupiedObject = null;
        }
    }
}
