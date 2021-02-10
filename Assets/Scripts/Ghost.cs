using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostType { SLOW, FAST };

public class Ghost : Character
{
    GhostType type;

    public void ghostInit(GridIndex index, GameController controller, GhostType type)
    {
        this.index = index;
        this.controller = controller;
        this.type = type;
        isMoving = false;
        int multiplier = type == GhostType.SLOW ? controller.SLOWGHOST_SPEED_MULTIPLIER : controller.FASTGHOST_SPEED_MULTIPLIER;
        speed = controller.BASE_SPEED * multiplier;
    }
}
