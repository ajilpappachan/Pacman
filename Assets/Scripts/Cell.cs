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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GridCell cellInit(GridIndex index, GameController controller)
    {
        this.index = index;
        this.controller = controller;
        cell = new GridCell(index, this);
        return cell;
    }

    public void spawnBrick()
    {
        if (cell.isActive) return;
        spawnedBrick = Instantiate(brickObject, spawner.transform.position, Quaternion.identity);
        spawnedBrick.transform.SetParent(spawner.transform);
    }

    public void destroyBrick()
    {
        if (!cell.isActive) return;
        Destroy(spawnedBrick);
    }

    public GameObject spawnObject(GameObject obj)
    {
        if (cell.isActive) { Debug.Log("IS ACTIVE"); return null; }
        occupiedObject = Instantiate(obj, spawner.transform.position, Quaternion.identity);
        return occupiedObject;
    }

    private void OnTriggerExit(Collider other)
    {
        Pacman pacman;
        if(other.TryGetComponent(out pacman))
        {
            controller.setCellActive(index);
            if(occupiedObject)
                occupiedObject = null;
        }
        if(other.gameObject == occupiedObject)
        {
            occupiedObject = null;
        }
    }
}
