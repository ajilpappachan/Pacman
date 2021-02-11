using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data Strucuture for grid coordinates for easier accessibility
public struct GridIndex
{
    public int x;
    public int y;
    public bool defined;

    //Initialise an undefined index
    public GridIndex(int define = 0)
    {
        x = 0;
        y = 0;
        defined = false;
    }

    //Initialise a defined index
    public GridIndex(int x, int y)
    {
        this.x = x;
        this.y = y;
        defined = true;
    }

    //String with cordinate values for debugging
    public string debug()
    {
        return (x + " " + y);
    }

    //Override equality operations
    public static bool operator== (GridIndex id1, GridIndex id2)
    {
        return (id1.x == id2.x && id1.y == id2.y);
    }

    public static bool operator!= (GridIndex id1, GridIndex id2)
    {
        return (id1.x != id2.y || id1.x != id2.y);
    }
}
