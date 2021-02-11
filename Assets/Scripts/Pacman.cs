using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : Character
{
    //Pacman and ghosts inherits key properties from a character class
    
    //Initialise Pacman
    public void pacmanInit(GridIndex index, GameController controller)
    {
        this.index = index;
        this.controller = controller;
        isMoving = false;
        nextDir = Vector2.zero;
        speed = controller.BASE_SPEED * controller.PACMAN_SPEED_MULTIPIER;
        animator = GetComponent<Animator>();
    }
}
