using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostType { SLOW, FAST };

public class Ghost : Character
{
    //Pacman and ghosts inherits key properties from a character class

    GhostType type;
    GridIndex target;
    int multiplier;

    //Initialise Ghost and set type
    public void ghostInit(GridIndex index, GameController controller, GhostType type)
    {
        this.index = index;
        this.controller = controller;
        this.type = type;
        isMoving = false;
        multiplier = (type == GhostType.SLOW) ? controller.SLOWGHOST_SPEED_MULTIPLIER : controller.FASTGHOST_SPEED_MULTIPLIER;
        speed = controller.BASE_SPEED * multiplier; //Adjust Speed based on type of ghost
        target = new GridIndex();
        StartCoroutine("findTarget");
    }

    //Find new target to move towards
    //If active walls are far away, move towards any random wall
    IEnumerator findTarget()
    {
        while(!target.defined)
        {
            target = controller.randomActiveCell();
            yield return null;
        }

        Vector2 distance = controller.getVector(index, target);
        if(distance.magnitude > 15.0f)
            target = controller.getRandomBoundary();
        goToTarget();
    }

    //!!!!Tried using A* algorithm. Causes Stack Overflow. Used a simpler alternative for prototyping!!!!
    //Go to the current target position, one cell at a time
    void goToTarget()
    {
        if (isMoving) return;
        if (target.y > index.y)
        {
            MoveGhost(0, 1);
            return;
        }
        if (target.y < index.y)
        {
            MoveGhost(0, -1);
            return;
        }
        if (target.x > index.x)
        {
            MoveGhost(1, 0);
            return;
        }
        if (target.x < index.x)
        {
            MoveGhost(-1, 0);
            return;
        }
    }

    //Move towards neighbouring cell
    //Continue to next cell
    //Find new target if target reached
    public void MoveGhost(int x, int y)
    {
        nextCell = new GridIndex(index.x + x, index.y + y);
        if (nextCell == target)
        {
            //If the next cell is an active cell, kill the player
            if(!controller.isResolved(nextCell) && controller.isActive(nextCell))
                controller.loseLife();
            target.defined = false;
            StartCoroutine("findTarget");
            return;
        }

        if (!controller.isCellActive(nextCell))
            StartCoroutine("moveGhost", (new Vector2(x, y)));
        else
        {
            target.defined = false;
            StartCoroutine("findTarget");
        }
    }

    //Move one cell according to speed
    IEnumerator moveGhost(Vector2 direction)
    {
        isMoving = true;
        float time = 0.0f;
        Vector3 startPos = transform.position;
        Vector3 destPos = transform.position + new Vector3(direction.x, 0.0f, direction.y);
        while (time < 1.0f)
        {
            transform.position = Vector3.Lerp(startPos, destPos, time);
            time += Time.deltaTime * speed;
            yield return null;
        }
        transform.position = destPos;
        index = nextCell;

        isMoving = false;
        goToTarget();
    }

    //Slow down ghost 50% if the power up is used
    public void slowSpeed(bool value)
    {
        if (value)
            speed = controller.BASE_SPEED * multiplier / 2;
        else
            speed = controller.BASE_SPEED * multiplier;

    }
}
