using UnityEngine;
using System.Collections.Generic;


public class MovementPath
{
    //List of all coordinate pairs where the path changes direction. To walk along a path, a unit moves in a straight line from one pivot coordinate to the next
    private List<Vector2Int> _pivots;

    public List<Vector2Int> Pivots => _pivots;

    /**
     * Generate a MovementPath object from the given List of Vector2Ints.
     */
    public MovementPath(List<Vector2Int> pivots) {
        _pivots = pivots;
    }
}
