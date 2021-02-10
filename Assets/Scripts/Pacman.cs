using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : Character
{
    public void pacmanInit(GridIndex index, GameController controller)
    {
        this.index = index;
        this.controller = controller;
        isMoving = false;
        speed = controller.BASE_SPEED * controller.PACMAN_SPEED_MULTIPIER;
        animator = GetComponent<Animator>();
    }
}
