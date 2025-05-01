using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TargetingUtility
{
    
    /**
     * <summary>Retrieves all Tiles in a given Grid that are targeted by the given properties.</summary>
     * <remarks>Only tiles that exist in the grid are returned.</remarks>
     * <param name="grid">The grid of tiles that will be retrieved</param>
     * <param name="origin">The Tile that this method begins its retrieval process on, and is included in the retrieval process.</param>
     * <param name="shape">The shape of tiles on the grid that will be retrieved. Can be Circle, Rectangle, Cone, Line.</param>
     * <param name="unit">The unit performing the move, used to get current direction and grid position</param>
     * <param name="primaryRange">The main factor in determining the size of the shape targeting property. Determines the diameter of a Circle, the side length of a Square, the altitude of a Cone, the length of a Line.</param>
     * <param name="secondaryRange">Allows the shape to vary in other factors. Optional parameter that defaults to -1. If the shape is a Circle, determines the radius of the inner circle that is not retrieved. If the shape is a Cone, determines the width of the cone.</param>
     */
    public static List<Tile> GetTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int origin, Shape shape, Unit unit, int primaryRange, int secondaryRange = -1)
    {
        List<Tile> targetedTiles = new List<Tile>();
        
        // Validate primary range
        if (primaryRange < 0)
            return targetedTiles;
            
        // Select the appropriate targeting method based on shape
        switch (shape)
        {
            case Shape.Circle:
                targetedTiles = GetCircleTiles(grid, origin, primaryRange, secondaryRange);
                break;
            case Shape.Square:
                targetedTiles = GetSquareTiles(grid, origin, primaryRange, unit.GetCurrentDirection());
                break;
            case Shape.Cone:
                targetedTiles = GetConeTiles(grid, origin, primaryRange, secondaryRange, unit.GetCurrentDirection());
                break;
            case Shape.Line:
                targetedTiles = GetLineTiles(grid, origin, primaryRange, unit.GetCurrentDirection());
                break;
        }
        
        return targetedTiles;
    }

    public static List<Tile> GetTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int origin, Unit unit, ScriptableMove move)
    {
        // Determine the actual origin based on the move's origin type
        Vector2Int actualOrigin = move.originType == ScriptableMove.OriginType.Self ? unit.GetGridPosition() : origin;
        return GetTiles(grid, actualOrigin, move.shape, unit, move.primaryRange, move.secondaryRange);
    }


// Helper method for Circle shape targeting
private static List<Tile> GetCircleTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int origin, int radius, int innerRadius)
{
    List<Tile> tiles = new List<Tile>();
    
    // Check each tile in a square area that encompasses the circle
    for (int x = origin.x - radius; x <= origin.x + radius; x++)
    {
        for (int y = origin.y - radius; y <= origin.y + radius; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            
            // Calculate distance from origin
            int distanceSquared = (x - origin.x) * (x - origin.x) + (y - origin.y) * (y - origin.y);
            
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
private static List<Tile> GetSquareTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int origin, int sideLength, Unit.Direction direction)
{
    List<Tile> tiles = new List<Tile>();
    
    // Calculate half side length for centered square
    int halfSide = sideLength / 2;
    
    // Determine the square area based on direction
    int startX, startY, endX, endY;
    
    switch (direction)
    {
        case Unit.Direction.North:
            startX = origin.x - halfSide;
            endX = origin.x + halfSide;
            startY = origin.y;
            endY = origin.y + sideLength - 1;
            break;
        case Unit.Direction.South:
            startX = origin.x - halfSide;
            endX = origin.x + halfSide;
            startY = origin.y - sideLength + 1;
            endY = origin.y;
            break;
        case Unit.Direction.East:
            startX = origin.x;
            endX = origin.x + sideLength - 1;
            startY = origin.y - halfSide;
            endY = origin.y + halfSide;
            break;
        case Unit.Direction.West:
            startX = origin.x - sideLength + 1;
            endX = origin.x;
            startY = origin.y - halfSide;
            endY = origin.y + halfSide;
            break;
        default: // For diagonal directions or if centered square is needed
            startX = origin.x - halfSide;
            endX = origin.x + halfSide;
            startY = origin.y - halfSide;
            endY = origin.y + halfSide;
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
private static List<Tile> GetConeTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int origin, int altitude, int width, Unit.Direction direction)
{
    List<Tile> tiles = new List<Tile>();
    
    // Set default width if not provided
    if (width <= 0)
        width = altitude;
    
    // Get direction vector based on the specified direction
    Vector2 dirVector = GetDirectionVector(direction);
    
    // Check each tile in a square area that encompasses the cone
    int checkRadius = altitude;
    for (int x = origin.x - checkRadius; x <= origin.x + checkRadius; x++)
    {
        for (int y = origin.y - checkRadius; y <= origin.y + checkRadius; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            Vector2 relativePos = new Vector2(x - origin.x, y - origin.y);
            
            // Skip the origin tile
            if (relativePos.magnitude == 0)
                continue;
            
            // Calculate distance along the direction vector
            float distanceAlongDir = Vector2.Dot(relativePos, dirVector);
            
            // Skip tiles behind the origin relative to direction
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
    
    // Add origin tile
    Tile originTile = grid[origin];
    if (originTile != null)
        tiles.Add(originTile);
    
    return tiles;
}

// Helper method for Line shape targeting
private static List<Tile> GetLineTiles(Dictionary<Vector2Int, Tile> grid, Vector2Int origin, int length, Unit.Direction direction)
{
    List<Tile> tiles = new List<Tile>();
    
    // Define direction vectors for each cardinal/ordinal direction
    Vector2Int dirVector = GetDirectionIntVector(direction);
    
    // Add tiles along the line
    for (int i = 0; i <= length; i++)
    {
        Vector2Int pos = origin + dirVector * i;
        if (!grid.ContainsKey(pos)) break;
        Tile tile = grid[pos];
        if (tile != null)
            tiles.Add(tile);
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
