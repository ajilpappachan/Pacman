using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    GridIndex index;
    GameController controller;
    float lifeDuration;

    //Initialise PowerUp object
    public void powerUpInit(GridIndex index, GameController controller, float lifeDuration)
    {
        this.index = index;
        this.controller = controller;
        this.lifeDuration = lifeDuration;
        StartCoroutine("lifetime"); //Start the lifetime clock
    }

    //Destroy powerup and start a new power up spawn timer
    IEnumerator lifetime()
    {
        yield return new WaitForSeconds(lifeDuration);
        controller.SpawnPowerUp();
        if (gameObject)
            Destroy(gameObject);
    }

    //Check if player touches the power up and use it if true
    private void OnTriggerEnter(Collider other)
    {
        Pacman pacman;
        if(other.TryGetComponent(out pacman))
        {
            controller.UsePowerUp();
            Destroy(gameObject);
        }
    }
}
