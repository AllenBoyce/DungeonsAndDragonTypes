//Some of this is based on: https://www.youtube.com/watch?v=kkAjpQAM-jE

using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float cellGap;

    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _camera;

    private Dictionary<Vector2Int, Tile> _grid = new Dictionary<Vector2Int, Tile>();


    [SerializeField] private GameObject _player;
    void Start()
    {

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
        SetCamera();
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

    private void GenerateGrid()
    {
        _grid = new Dictionary<Vector2Int, Tile>();
        float xOffset = (_width - 1) * (1 + cellGap) / 2;
        float yOffset = (_height - 1) * (1 + cellGap) / 2;


        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {

                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x * (1 + cellGap) - xOffset, y * (1 + cellGap) - yOffset), Quaternion.identity);

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
