using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TargetingUtility
{
    
    /**
     * <summary>Retrieves all Tiles in a given Grid that are targeted by the given properties.</summary>
     * <remarks>Only tiles that exist in the grid are returned.</remarks>
     * <param name="grid">The grid of tiles that will be retrieved</param>
     * <param name="targeted">The Tile that this method begins its retrieval process on, and is included in the retrieval process. Can also be thought of as the "targeted" or "center" of the shape.</param>
     * <param name="shape">The shape of tiles on the grid that will be retrieved. Can be Circle, Rectangle, Cone, Line.</param>
     * <param name="unit">The unit performing the move, used to get current direction and grid position</param>
     * <param name="primaryRange">The main factor in determining the size of the shape targeting property. Determines the diameter of a Circle, the side length of a Square, the altitude of a Cone, the length of a Line.</param>
     * <param name="secondaryRange">Allows the shape to vary in other factors. Optional parameter that defaults to -1. If the shape is a Circle, determines the radius of the inner circle that is not retrieved. If the shape is a Cone, determines the width of the cone.</param>
     */
    public static List<Tile> GetTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int targeted, Shape shape, Unit unit, int primaryRange, int secondaryRange = -1)
    {
        List<Tile> targetedTiles = new List<Tile>();
        
        // Validate primary range
        if (primaryRange < 0)
            return targetedTiles;
            
        // Select the appropriate targeting method based on shape
        switch (shape)
        {
            case Shape.Circle:
                targetedTiles = GetCircleTiles(grid, targeted, primaryRange, secondaryRange);
                break;
            case Shape.Square:
                targetedTiles = GetSquareTiles(grid, targeted, primaryRange, unit.GetCurrentDirection());
                break;
            case Shape.Cone:
                targetedTiles = GetConeTiles(grid, targeted, primaryRange, secondaryRange, unit.GetCurrentDirection());
                break;
            case Shape.Line:
                targetedTiles = GetLineTiles(grid, targeted, primaryRange, unit.GetGridPosition());
                break;
        }
        
        return targetedTiles;
    }

    public static List<Tile> GetTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int targeted, Unit unit, ScriptableMove move)
    {
        // For line targeting, we want to draw a line from the unit to the targeted tile
        if (move.shape == Shape.Line) {
            return GetLineTiles(grid, targeted, move.primaryRange, unit.GetGridPosition());
        }
        
        // For other shapes, determine the actual targeted tile based on the move's origin type
        Vector2Int actualTargeted = move.targetType == ScriptableMove.TargetType.Self ? unit.GetGridPosition() : targeted;
        return GetTiles(grid, actualTargeted, move.shape, unit, move.primaryRange, move.secondaryRange);
    }


// Helper method for Circle shape targeting
private static List<Tile> GetCircleTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int targeted, int radius, int innerRadius)
{
    List<Tile> tiles = new List<Tile>();
    
    // Check each tile in a square area that encompasses the circle
    for (int x = targeted.x - radius; x <= targeted.x + radius; x++)
    {
        for (int y = targeted.y - radius; y <= targeted.y + radius; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            
            // Calculate distance from targeted
            int distanceSquared = (x - targeted.x) * (x - targeted.x) + (y - targeted.y) * (y - targeted.y);
            
            // Add tile if it's within the outer circle and outside the inner circle
            bool withinOuterCircle = distanceSquared <= radius * radius;
            bool outsideInnerCircle = innerRadius <= 0 || distanceSquared > (innerRadius - 1) * (innerRadius - 1);
            
            if (withinOuterCircle && outsideInnerCircle)
            {
                if (!grid.ContainsKey(pos)) break;
                Tile tile = grid[pos];
                if (tile != null)
                    tiles.Add(tile);
            }
        }
    }
    
    return tiles;
}

// Helper method for Square shape targeting
private static List<Tile> GetSquareTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int targeted, int sideLength, Unit.Direction direction)
{
    List<Tile> tiles = new List<Tile>();
    
    // Calculate half side length for centered square
    int halfSide = sideLength / 2;
    
    // Determine the square area based on direction
    int startX, startY, endX, endY;
    
    switch (direction)
    {
        case Unit.Direction.North:
            startX = targeted.x - halfSide;
            endX = targeted.x + halfSide;
            startY = targeted.y;
            endY = targeted.y + sideLength - 1;
            break;
        case Unit.Direction.South:
            startX = targeted.x - halfSide;
            endX = targeted.x + halfSide;
            startY = targeted.y - sideLength + 1;
            endY = targeted.y;
            break;
        case Unit.Direction.East:
            startX = targeted.x;
            endX = targeted.x + sideLength - 1;
            startY = targeted.y - halfSide;
            endY = targeted.y + halfSide;
            break;
        case Unit.Direction.West:
            startX = targeted.x - sideLength + 1;
            endX = targeted.x;
            startY = targeted.y - halfSide;
            endY = targeted.y + halfSide;
            break;
        default: // For diagonal directions or if centered square is needed
            startX = targeted.x - halfSide;
            endX = targeted.x + halfSide;
            startY = targeted.y - halfSide;
            endY = targeted.y + halfSide;
            break;
    }
    
    // Add tiles within the square area
    for (int x = startX; x <= endX; x++)
    {
        for (int y = startY; y <= endY; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (!grid.ContainsKey(pos)) break;
            Tile tile = grid[pos];
            if (tile != null)
                tiles.Add(tile);
        }
    }
    
    return tiles;
}

// Helper method for Cone shape targeting
private static List<Tile> GetConeTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int targeted, int altitude, int width, Unit.Direction direction)
{
    List<Tile> tiles = new List<Tile>();
    
    // Set default width if not provided
    if (width <= 0)
        width = altitude;
    
    // Get direction vector based on the specified direction
    Vector2 dirVector = GetDirectionVector(direction);
    
    // Check each tile in a square area that encompasses the cone
    int checkRadius = altitude;
    for (int x = targeted.x - checkRadius; x <= targeted.x + checkRadius; x++)
    {
        for (int y = targeted.y - checkRadius; y <= targeted.y + checkRadius; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            Vector2 relativePos = new Vector2(x - targeted.x, y - targeted.y);
            
            // Skip the targeted tile itself
            if (relativePos.magnitude == 0)
                continue;
            
            // Calculate distance along the direction vector
            float distanceAlongDir = Vector2.Dot(relativePos, dirVector);
            
            // Skip tiles behind the targeted tile relative to direction
            if (distanceAlongDir <= 0)
                continue;
            
            // Calculate distance perpendicular to the direction vector
            Vector2 projectedPos = dirVector * distanceAlongDir;
            float perpendicularDistance = Vector2.Distance(relativePos, projectedPos);
            
            // Check if the tile is within the cone
            // Width of cone at this distance = (width * distanceAlongDir / altitude)
            float halfConeWidth = (width * 0.5f * distanceAlongDir) / altitude;
            
            if (distanceAlongDir <= altitude && perpendicularDistance <= halfConeWidth)
            {
                if (!grid.ContainsKey(pos)) break;
                Tile tile = grid[pos];
                if (tile != null)
                    tiles.Add(tile);
            }
        }
    }
    
    // Add targeted tile itself
    Tile targetedTile = grid[targeted];
    if (targetedTile != null)
        tiles.Add(targetedTile);
    
    return tiles;
}

// Helper method for Line shape targeting
private static List<Tile> GetLineTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int targeted, int length, Vector2Int unitPosition)
{
    List<Tile> tiles = new List<Tile>();
    Debug.Log("GetLineTiles: " + targeted + " " + unitPosition);
    
    // Calculate direction vector from unit to target
    Vector2Int direction = targeted - unitPosition;
    float distance = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
    
    // If distance is 0, just return the targeted tile
    if (distance == 0)
    {
        if (grid.ContainsKey(targeted))
            tiles.Add(grid[targeted]);
        return tiles;
    }
    
    // Normalize direction vector
    Vector2 normalizedDir = new Vector2(direction.x / distance, direction.y / distance);
    
    // Calculate the actual length to use (minimum of requested length and actual distance)
    float actualLength = Mathf.Min(length, distance);
    
    // Calculate end point based on length
    Vector2 endPoint = new Vector2(unitPosition.x + normalizedDir.x * actualLength,
                                 unitPosition.y + normalizedDir.y * actualLength);
    
    // Use Bresenham's line algorithm to find all tiles in the line
    int x0 = unitPosition.x;
    int y0 = unitPosition.y;
    int x1 = Mathf.RoundToInt(endPoint.x);
    int y1 = Mathf.RoundToInt(endPoint.y);
    
    int dx = Mathf.Abs(x1 - x0);
    int dy = Mathf.Abs(y1 - y0);
    int sx = x0 < x1 ? 1 : -1;
    int sy = y0 < y1 ? 1 : -1;
    int err = dx - dy;
    
    while (true)
    {
        Vector2Int currentPos = new Vector2Int(x0, y0);
        if (grid.ContainsKey(currentPos))
        {
            Tile tile = grid[currentPos];
            if (tile != null)
                tiles.Add(tile);
        }
        
        if (x0 == x1 && y0 == y1) break;
        
        int e2 = 2 * err;
        if (e2 > -dy)
        {
            err -= dy;
            x0 += sx;
        }
        if (e2 < dx)
        {
            err += dx;
            y0 += sy;
        }
    }
    
    return tiles;
}

// Helper method to convert Unit.Direction enum to Vector2
private static Vector2 GetDirectionVector(Unit.Direction direction)
{
    switch (direction)
    {
        case Unit.Direction.North:
            return Vector2.up;
        case Unit.Direction.NorthEast:
            return new Vector2(0.7071f, 0.7071f); // Normalized vector (1,1)
        case Unit.Direction.East:
            return Vector2.right;
        case Unit.Direction.SouthEast:
            return new Vector2(0.7071f, -0.7071f); // Normalized vector (1,-1)
        case Unit.Direction.South:
            return Vector2.down;
        case Unit.Direction.SouthWest:
            return new Vector2(-0.7071f, -0.7071f); // Normalized vector (-1,-1)
        case Unit.Direction.West:
            return Vector2.left;
        case Unit.Direction.NorthWest:
            return new Vector2(-0.7071f, 0.7071f); // Normalized vector (-1,1)
        default:
            return Vector2.up;
    }
}

// Helper method to convert Unit.Direction enum to Vector2Int
public static Vector2Int GetDirectionIntVector(Unit.Direction direction)
{
    switch (direction)
    {
        case Unit.Direction.North:
            return new Vector2Int(0, 1);
        case Unit.Direction.NorthEast:
            return new Vector2Int(1, 1);
        case Unit.Direction.East:
            return new Vector2Int(1, 0);
        case Unit.Direction.SouthEast:
            return new Vector2Int(1, -1);
        case Unit.Direction.South:
            return new Vector2Int(0, -1);
        case Unit.Direction.SouthWest:
            return new Vector2Int(-1, -1);
        case Unit.Direction.West:
            return new Vector2Int(-1, 0);
        case Unit.Direction.NorthWest:
            return new Vector2Int(-1, 1);
        default:
            return new Vector2Int(0, 1);
    }
}


    public enum Shape
    {
        Circle,
        Square,
        Cone,
        Line
    }
}
