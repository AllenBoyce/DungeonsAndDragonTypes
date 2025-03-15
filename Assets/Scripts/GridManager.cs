//Some of this is based on: https://www.youtube.com/watch?v=kkAjpQAM-jE

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float cellGap;

    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _camera;

    [SerializeField] private List<Sprite> _tileSprites;

    private Dictionary<Vector2Int, Tile> _grid = new Dictionary<Vector2Int, Tile>();

    private string _gridMap = "TL T T T T T T T T T T T T T T TR " +
                               "SL - - - - - - - - - - - - X - SR " +
                               "SL - - - X - - - - - - - - - - SR " +
                               "SL - - - - - - - - - - - X - - SR " +
                               "SL - X - - - - - - - - - - - - SR " +
                               "SL - - X X - - - - - - - - - - SR " +
                               "SL - - - X - - - - X - - - - - SR " +
                               "SL - - - - - - - - X - - - - - SR " +
                               "BL B B B B B B B B B B B B B B BR";
    //private Dictionary<string, string> _tileKeyMap = new Dictionary<string, string>();
    private Dictionary<string, int> _tileSpriteMap = new Dictionary<string, int>();
    
    //private Tilemap _tilemap = Resources.Load<Tilemap>("Sprites/Tilesheets");


    [SerializeField] private GameObject _player;
    void Start()
    {
        /*//Populate tileKeyMap
        _tileKeyMap.Add("-", "Apple-Trimmed-Tilesheet-34");
        _tileKeyMap.Add("X", "Apple-Trimmed-Tilesheet-37");
        _tileKeyMap.Add("TL", "Apple-Trimmed-Tilesheet-3");
        _tileKeyMap.Add("TR", "Apple-Trimmed-Tilesheet-5");
        _tileKeyMap.Add("BL", "Apple-Trimmed-Tilesheet-63");
        _tileKeyMap.Add("BR", "Apple-Trimmed-Tilesheet-65");
        _tileKeyMap.Add("B", "Apple-Trimmed-Tilesheet-64");
        _tileKeyMap.Add("T", "Apple-Trimmed-Tilesheet-4");
        _tileKeyMap.Add("SL", "Apple-Trimmed-Tilesheet-33");
        _tileKeyMap.Add("SR", "Apple-Trimmed-Tilesheet-35");*/
        
        _tileSpriteMap.Add("-", 4);
        _tileSpriteMap.Add("X", 6);
        _tileSpriteMap.Add("TL", 0);
        _tileSpriteMap.Add("TR", 2);
        _tileSpriteMap.Add("BL", 8);
        _tileSpriteMap.Add("BR", 10);
        _tileSpriteMap.Add("B", 9);
        _tileSpriteMap.Add("T", 1);
        _tileSpriteMap.Add("SL", 3);
        _tileSpriteMap.Add("SR", 5);
        

        var gridComponent = GetComponent<Grid>();
        if(gridComponent != null)
        {
            //Assuming cellGap is a Vector2 with x and y being the same
            cellGap = gridComponent.cellGap.x;
            if(cellGap != gridComponent.cellGap.y)
            {
                Debug.LogError("Grid does not have uniform cell gap");
            }
        }
        else
        {
            Debug.LogError("Grid component not found");
        }



        GenerateGrid();

        //TEMPORARY: Manual tile type setting
        //Obstacle tiles at: (1,7), (2,3), (5, 7), (6, 2), (11, 1), (12, 3), (14, 6)
        _grid[new Vector2Int(1, 7)].blocksMovement = true;
        _grid[new Vector2Int(2, 3)].blocksMovement = true;
        _grid[new Vector2Int(5, 7)].blocksMovement = true;
        _grid[new Vector2Int(6, 2)].blocksMovement = true;
        _grid[new Vector2Int(11, 1)].blocksMovement = true;
        _grid[new Vector2Int(12, 3)].blocksMovement = true;
        _grid[new Vector2Int(14, 6)].blocksMovement = true;
        _grid[new Vector2Int(13, 4)].blocksMovement = true;
        _grid[new Vector2Int(14, 4)].blocksMovement = true;

        //Now do the outline of the grid
        for (int i = 0; i < 16; i++)
        {
            _grid[new Vector2Int(i, 0)].blocksMovement = true;
            _grid[new Vector2Int(i, 8)].blocksMovement = true;
        }
        for (int i = 0; i < 9; i++)
        {
            _grid[new Vector2Int(0,i)].blocksMovement = true;
            _grid[new Vector2Int(15,i)].blocksMovement = true;
        }

        SetCamera();
    }

    public Dictionary<Vector2Int, Tile> Grid {
        get {
            return _grid;
        }
    }

    public MovementPath GenerateMovementPath(Vector2Int origin, Vector2Int destination)
    {
        var path = FindPath(origin, destination);
        if (path == null) return null;

        var pivots = GetPivotPoints(path);

        var movementPath = new MovementPath(pivots);
        //movementPath.Pivots = pivots;
        return movementPath;
    }

    private List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
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

            foreach (var neighbor in GetValidNeighbors(current))
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

    private List<Vector2Int> GetValidNeighbors(Vector2Int pos)
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
            if (_grid.ContainsKey(neighbor) && !_grid[neighbor].blocksMovement)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    private float ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private Vector2Int GetLowestFScore(List<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
    {
        var lowest = openSet[0];
        foreach (var pos in openSet)
        {
            if (fScore.GetValueOrDefault(pos, float.MaxValue) < fScore.GetValueOrDefault(lowest, float.MaxValue))
                lowest = pos;
        }
        return lowest;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private List<Vector2Int> GetPivotPoints(List<Vector2Int> path)
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

    //Note: Untested
    public Vector2 GetGridPositionFromGameCoordinates(Vector2Int coordinates)
    {
        return _grid[coordinates].transform.position;
    }


    //public float GridXFromGameX(float x)
    //{
    //    float xOffset = (_width - 1) * (1 + cellGap) / 2;
    //    return (x + xOffset) / (1 + cellGap);
    //}

    //public float GridYFromGameY(float y)
    //{
    //    float yOffset = (_height - 1) * (1 + cellGap) / 2;
    //    return (y + yOffset) / (1 + cellGap);
    //}

    //Note: Untested
    public Vector2 GetWorldPosition(Vector2Int position)
    {
        float x = GetWorldX(position.x);
        float y = GetWorldY(position.y);

        return new Vector2(x, y);
    }

    //Game coordinates, not grid cells
    public float GetWorldX(int x)
    {
        float xOffset = (_width - 1) * (1 + cellGap) / 2;
        return x * (1 + cellGap) - xOffset;
    }

    public float GetWorldY(int y)
    {
        float yOffset = (_height - 1) * (1 + cellGap) / 2;
        return y * (1 + cellGap) - yOffset;
    }

    public int GetGridX(float worldX)
    {
        float xOffset = (_width - 1) * (1 + cellGap) / 2;
        return Mathf.RoundToInt((worldX + xOffset) / (1 + cellGap));
    }

    public int GetGridY(float worldY)
    {
        float yOffset = (_height - 1) * (1 + cellGap) / 2;
        return Mathf.RoundToInt((worldY + yOffset) / (1 + cellGap));
    }

    public Vector2Int GetGridPosition(Vector2 worldPosition)
    {
        return new Vector2Int(
            GetGridX(worldPosition.x),
            GetGridY(worldPosition.y)
        );
    }

    private void GenerateGrid()
    {
        _grid = new Dictionary<Vector2Int, Tile>();
        float xOffset = (_width - 1) * (1 + cellGap) / 2;
        float yOffset = (_height - 1) * (1 + cellGap) / 2;

        string[] tileTypes = _gridMap.Split();
        int i = 0;
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x * (1 + cellGap) - xOffset, y * (1 + cellGap) - yOffset), Quaternion.identity);
                /*string tileType = tileTypes[i];
                
                string tileResourceName = _tileKeyMap[tileType]; // Add this line
                var tile = Resources.Load<UnityEngine.Tilemaps.Tile>($"Sprites/Tilesheets/Palette/{tileResourceName}");
                Sprite tileSprite = tile.sprite;*/

                Sprite tileSprite = _tileSprites[_tileSpriteMap[tileTypes[i]]];
                Debug.Log(tileSprite.name);
                Debug.Log(spawnedTile.name);
                spawnedTile.SetSprite(tileSprite);
                i++;

                //Make sure these are rendered above the tile sprites
                Vector3 tilePos = spawnedTile.transform.position;
                tilePos.z = -4;
                spawnedTile.transform.position = tilePos;

                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.x = x;
                spawnedTile.y = y;

                _grid[new Vector2Int(x, y)] = spawnedTile;
            }
        }
        //_camera.transform.position = new Vector3(0, 0, -10);
        //_camera.transform.position = new Vector3((_width * (1 + cellGap)) / 2 - (1 + cellGap) / 2, (_height * (1 + cellGap)) / 2 - (1 + cellGap) / 2, -10);

    }

    //Credit to Copilot
    void SetCamera()
    {
        // Calculate the width and height of the grid including the cell gap
        float gridWidth = _width * (1 + cellGap);
        float gridHeight = _height * (1 + cellGap);

        // Calculate the aspect ratio of the camera
        float aspectRatio = (float)Screen.width / Screen.height;

        // Set the camera's orthographic size based on the larger dimension of the grid
        if (gridWidth / aspectRatio > gridHeight)
        {
            Camera.main.orthographicSize = gridWidth / (2 * aspectRatio);
        }
        else
        {
            Camera.main.orthographicSize = gridHeight / 2;
        }

        // Center the camera on the grid
        _camera.position = new Vector3(0, 0, -10);
    }

    //private void GenerateGrid()
    //{
    //    _grid = new Dictionary<Vector2Int, Tile>();
    //    for (int x = 0; x < _width; x++)
    //    {
    //        for (int y = 0; y < _height; y++)
    //        {
    //            var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
    //            spawnedTile.name = $"Tile {x} {y}";

    //            _grid[new Vector2Int(x, y)] = spawnedTile;
    //        }
    //    }
    //    //_camera.transform.position = new Vector3((float)_width /2 -0.5f, (float)_height / 2 - 0.5f, -10);
    //    _camera.transform.position = new Vector3((_width * (1 + cellGap)) / 2 - (1 + cellGap) / 2, (_height * (1 + cellGap)) / 2 - (1 + cellGap) / 2, -10);

    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
