using System.Collections.Generic;
using UnityEngine;

public static class MovementUtility
{
    public static MovementPath GenerateMovementPath(Dictionary<Vector2Int, Tile> grid, Vector2Int origin, Vector2Int destination)
    {
        var path = FindPath(grid, origin, destination);
        if (path == null) return null;

        var pivots = GetPivotPoints(path);

        var movementPath = new MovementPath(pivots);
        //movementPath.Pivots = pivots;
        return movementPath;
    }

    private static List<Vector2Int> FindPath(Dictionary<Vector2Int, Tile> grid, Vector2Int start, Vector2Int end)
    {
        var openSet = new List<Vector2Int>();
        var closedSet = new HashSet<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, float>();
        var fScore = new Dictionary<Vector2Int, float>();

        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = ManhattanDistance(start, end);

        while (openSet.Count > 0)
        {
            var current = GetLowestFScore(openSet, fScore);
            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in GetValidNeighbors(grid, current))
            {
                if (closedSet.Contains(neighbor)) continue;

                var tentativeGScore = gScore[current] + 1;

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= gScore.GetValueOrDefault(neighbor, float.MaxValue))
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + ManhattanDistance(neighbor, end);
            }
        }

        return null;
    }

    private static List<Vector2Int> GetValidNeighbors(Dictionary<Vector2Int, Tile> grid, Vector2Int pos)
    {
        var neighbors = new List<Vector2Int>();
        var directions = new Vector2Int[] {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        foreach (var dir in directions)
        {
            var neighbor = pos + dir;
            if (grid.ContainsKey(neighbor) && !grid[neighbor].blocksMovement)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    private static float ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static Vector2Int GetLowestFScore(List<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
    {
        var lowest = openSet[0];
        foreach (var pos in openSet)
        {
            if (fScore.GetValueOrDefault(pos, float.MaxValue) < fScore.GetValueOrDefault(lowest, float.MaxValue))
                lowest = pos;
        }
        return lowest;
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private static List<Vector2Int> GetPivotPoints(List<Vector2Int> path)
    {
        if (path.Count <= 2) return path;

        var pivots = new List<Vector2Int> { path[0] };
        var currentDirection = path[1] - path[0];

        for (int i = 1; i < path.Count - 1; i++)
        {
            var newDirection = path[i + 1] - path[i];
            if (newDirection != currentDirection)
            {
                pivots.Add(path[i]);
                currentDirection = newDirection;
            }
        }

        pivots.Add(path[path.Count - 1]);
        return pivots;
    }
    
    //Determines the cardinal direction that the unit at point Origin must face in order to be closest to facing
    //The destination point.
    //Returns Unit.Direction enum North, NorthEast, East, SouthEast, South, SouthWest, West, NorthEast
    public static Unit.Direction GetDirection(Vector2Int origin, Vector2Int destination)
    {
        float dx = destination.x - origin.x;
        float dy = destination.y - origin.y;
        
        // Calculate the angle in radians
        float angle = Mathf.Atan2(dy, dx);
        
        // Convert to degrees and ensure it's between 0 and 360
        float degrees = angle * Mathf.Rad2Deg;
        if (degrees < 0) degrees += 360f;
        
        // Map the angle to 8 directions (each covering 45 degrees)
        if (degrees >= 337.5f || degrees < 22.5f)
            return Unit.Direction.East;
        else if (degrees >= 22.5f && degrees < 67.5f)
            return Unit.Direction.NorthEast;
        else if (degrees >= 67.5f && degrees < 112.5f)
            return Unit.Direction.North;
        else if (degrees >= 112.5f && degrees < 157.5f)
            return Unit.Direction.NorthWest;
        else if (degrees >= 157.5f && degrees < 202.5f)
            return Unit.Direction.West;
        else if (degrees >= 202.5f && degrees < 247.5f)
            return Unit.Direction.SouthWest;
        else if (degrees >= 247.5f && degrees < 292.5f)
            return Unit.Direction.South;
        else // degrees >= 292.5f && degrees < 337.5f
            return Unit.Direction.SouthEast;
    }
}
