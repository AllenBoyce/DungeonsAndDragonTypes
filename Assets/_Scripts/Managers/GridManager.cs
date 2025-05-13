//Some of this is based on: https://www.youtube.com/watch?v=kkAjpQAM-jE

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
/**
 * GridManager is responsible for the Grid the Units exist on.
 * It generates a Grid of Tile objects, and positions the camera.
 */
public class GridManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /*
     * cellGap: the distance between the tiles horizontally and vertically.
     * 1: distance between tiles is equivalent to the length/width of one cell
     * 0: no distance between tiles
     */
    [SerializeField] private float _cellGapPercentage = 0.025f; // 5% of tile size
    private float _cellGap;

    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _camera;

    [SerializeField] private List<Sprite> _tileSprites;

    //Format: <(x,y), Tile>
    public Dictionary<Vector2Int, Tile> _grid = new Dictionary<Vector2Int, Tile>();
    private string gridMap1 = "TL T T T T T T T T T T T T T T TR " +
                               "SL - - - - - - - - - - - - X - SR " +
                               "SL - - - X - - - - - - - - - - SR " +
                               "SL - - - - - - - - - - - X - - SR " +
                               "SL - X - - - - - - - - - - - - SR " +
                               "SL - - X X - - - - - - - - - - SR " +
                               "SL - - - X - - - - X - - - - - SR " +
                               "SL - - - - - - - - X - - - - - SR " +
                               "BL B B B B B B B B B B B B B B BR";
    private string gridMap2 = "TL T T T T T T T T T T T T T T TR " +
                               "SL - - - - - - - X - - - - - - SR " +
                               "SL - - - - - - - X - - - - - - SR " +
                               "SL - - - - - - - - - - - - - - SR " +
                               "SL - - - - - - - - - - - - - - SR " +
                               "SL - - - - - - - - - - - - - - SR " +
                               "SL - - - - - - - X - - - - - - SR " +
                               "SL - - - - - - - X - - - - - - SR " +
                               "BL B B B B B B B B B B B B B B BR";
    private string gridMap3 = "TL T T T T T T T T T T T T T T TR " +
                               "SL - - - - - - - - - - - - - - SR " +
                               "SL - - - X - - - - - - X - - - SR " +
                               "SL - - - - - - - - - - - - - - SR " +
                               "SL - X - - - - - X - - - - X - SR " +
                               "SL - - - - - - - - - - - - - - SR " +
                               "SL - - - X - - - - - - X - - - SR " +
                               "SL - - - - - - - - - - - - - - SR " +
                               "BL B B B B B B B B B B B B B B BR";

    
    private string _gridMap;
    private Dictionary<string, int> _tileSpriteMap = new Dictionary<string, int>();


    [SerializeField] private GameObject _player;
    void Start()
    {
        // Calculate cell gap based on screen size
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;
        float screenAspect = screenWidth / screenHeight;
        
        // Use the smaller dimension to ensure consistent gaps
        float baseSize = Mathf.Min(screenWidth, screenHeight);
        //_cellGap = baseSize * _cellGapPercentage / 100f;
        _cellGap = 0.025f;
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

        List<string> gridMaps = new List<string> { gridMap1, gridMap2, gridMap3 };
        _gridMap = gridMaps[Random.Range(0, gridMaps.Count)];

        GenerateGrid();

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
    public int Width { get { return _width; } }
    public int Height { get { return _height; } }

    #region Coordinate Conversion

    //Note: Untested
    public Vector2 GetGridPositionFromGameCoordinates(Vector2Int coordinates)
    {
        return _grid[coordinates].transform.position;
    }
    
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
        float xOffset = (_width - 1) * (1 + _cellGap) / 2;
        return x * (1 + _cellGap) - xOffset;
    }

    public float GetWorldY(int y)
    {
        float yOffset = (_height - 1) * (1 + _cellGap) / 2;
        return y * (1 + _cellGap) - yOffset;
    }

    public int GetGridX(float worldX)
    {
        float xOffset = (_width - 1) * (1 + _cellGap) / 2;
        return Mathf.RoundToInt((worldX + xOffset) / (1 + _cellGap));
    }

    public int GetGridY(float worldY)
    {
        float yOffset = (_height - 1) * (1 + _cellGap) / 2;
        return Mathf.RoundToInt((worldY + yOffset) / (1 + _cellGap));
    }

    public Vector2Int GetGridPosition(Vector2 worldPosition)
    {
        return new Vector2Int(
            GetGridX(worldPosition.x),
            GetGridY(worldPosition.y)
        );
    }
    
    #endregion

    private void GenerateGrid()
    {
        _grid = new Dictionary<Vector2Int, Tile>();
        float xOffset = (_width - 1) * (1 + _cellGap) / 2;
        float yOffset = (_height - 1) * (1 + _cellGap) / 2;

        string[] tileTypes = _gridMap.Split();
        int i = 0;
        for (int y = _height - 1; y >= 0; y--)
        {
            for (int x = 0; x < _width; x++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x * (1 + _cellGap) - xOffset, y * (1 + _cellGap) - yOffset), Quaternion.identity);
                spawnedTile.transform.parent = transform;

                Sprite tileSprite = _tileSprites[_tileSpriteMap[tileTypes[i]]];
                spawnedTile.SetSprite(tileSprite);
                if (tileTypes[i].Equals("X")) { spawnedTile.blocksMovement = true; }
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
        float gridWidth = _width * (1 + _cellGap);
        float gridHeight = _height * (1 + _cellGap);

        // Calculate the aspect ratio of the camera
        float aspectRatio = 16f / 9f;

        // Set the camera's orthographic size based on the larger dimension of the grid
        float orthographicSize;
        if (gridWidth / aspectRatio > gridHeight)
        {
            orthographicSize = gridWidth / (2 * aspectRatio) * 1.1f;
        }
        else
        {
            orthographicSize = gridHeight / 2 * 1.1f;
        }
        Camera.main.orthographicSize = orthographicSize;
        Camera.main.aspect = aspectRatio;
        
        // Calculate the extra space added by the 1.1f multiplier
        float extraSpace = orthographicSize * 0.1f;
        
        // Center the camera on the grid horizontally, but offset vertically to align with top
        _camera.position = new Vector3(0, -extraSpace, Constants.CAMERA_LAYER);
    }

    public Vector2Int GetSpawnPosition(int player) {
        //If player is 0, spawn in the left side of the grid. If player is 1, spawn in the right side of the grid.
        //Find a random tile in the proper side of the grid, that does not yet have a unit on it, nor does it block movement.
        if(player == 0) {
            List<Vector2Int> possibleSpawns = new List<Vector2Int>();
            foreach (Vector2Int position in _grid.Keys) {
                Debug.Log("GridManager GetSpawnPosition: " + position + " " + _grid[position].name + " " + _grid[position].blocksMovement);
                if (position.x < _width / 2 && !GameManager.Instance.IsTileOccupied(position) && !_grid[position].blocksMovement) {
                    possibleSpawns.Add(position);
                }
            }
            return possibleSpawns[Random.Range(0, possibleSpawns.Count)];
        }
        else {
            List<Vector2Int> possibleSpawns = new List<Vector2Int>();
            foreach (Vector2Int position in _grid.Keys) {
                if (position.x >= _width / 2 && !GameManager.Instance.IsTileOccupied(position) && !_grid[position].blocksMovement) {
                    possibleSpawns.Add(position);
                }
            }
            return possibleSpawns[Random.Range(0, possibleSpawns.Count)];
        }
    }

}
