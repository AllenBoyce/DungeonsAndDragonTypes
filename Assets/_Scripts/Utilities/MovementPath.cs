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
    public int Distance() {
        if (_pivots == null || _pivots.Count <= 1) {
            return 0;
        }

        int distance = 0;
        for (int i = 1; i < _pivots.Count; i++) {
            Vector2Int start = _pivots[i - 1];
            Vector2Int end = _pivots[i];
            distance += Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);
        }
        return distance;
    }
    public List<Vector2Int> ComprisingTiles() {
        List<Vector2Int> tiles = new List<Vector2Int>();
        
        if (_pivots == null || _pivots.Count == 0) {
            return tiles;
        }

        // Add the first pivot
        tiles.Add(_pivots[0]);

        // For each subsequent pivot, add all tiles along the line to it
        for (int i = 1; i < _pivots.Count; i++) {
            Vector2Int start = _pivots[i - 1];
            Vector2Int end = _pivots[i];
            
            // Calculate direction and distance
            Vector2Int direction = new Vector2Int(
                Mathf.Clamp(end.x - start.x, -1, 1),
                Mathf.Clamp(end.y - start.y, -1, 1)
            );
            
            // Add all tiles along the line
            Vector2Int current = start + direction;
            while (current != end) {
                tiles.Add(current);
                current += direction;
            }
            
            // Add the end pivot
            tiles.Add(end);
        }

        return tiles;
    }
}
