using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using UnityEngine.UI;

public static class Allocator
{
    public static Dictionary<Vector3Int,MovementComponent> MoveableObjects = new Dictionary<Vector3Int, MovementComponent>();

    public static Grid grid;

    [RuntimeInitializeOnLoadMethod]
    public static void FindGrid()
    {
        grid = GameObject.FindObjectOfType<Grid>();//будут проблемы, если на сцене несколько 
        
    }



}
