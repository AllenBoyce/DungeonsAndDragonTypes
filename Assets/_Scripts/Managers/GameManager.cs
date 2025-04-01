using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //private GameBaseState _currentState;
    private GameState _currentState;
    private UnitManager _unitManager;
    private LevelManager _levelManager;
    private GridManager _gridManager;
    private MovementController _movementController;
    
    //Variables pertaining to the game
    private int _activePlayer;
    private Unit _selectedUnit;

    private bool DEBUG = true;
    private bool DEMO = true;
    void Start()
    {
        _unitManager = FindFirstObjectByType<UnitManager>();
        _levelManager = FindFirstObjectByType<LevelManager>();
        _gridManager = FindFirstObjectByType<GridManager>();
        _movementController = FindFirstObjectByType<MovementController>();


        if (DEMO)
        {
            Unit u = _unitManager.GenerateUnit("Garchomp", 0);
            _levelManager.PutUnit(u, 5, 5);
            _selectedUnit = u;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ClickCheck();
    }

    private void TransitionState(GameState nextState)
    {
        Debug.Log("Transitioning State to: " + nextState);
        _currentState = nextState;
    }

    #region Check and Logic Regarding Mouse Clicks
    private void ClickCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = MousePosition();
            //Debug.Log(mousePosition);
            HandleLeftClick(mousePosition);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }

    private void HandleLeftClick(Vector2 mousePosition)
    {
        Vector2Int mouseTile = new Vector2Int(_gridManager.GetGridX(mousePosition.x), _gridManager.GetGridY(mousePosition.y));
        if (IsTileWithinBounds(mouseTile))
        {
            HandleTileClicked(mouseTile);
        }
        else
        {
            //Logic for clicking outside of tile
        }
    }

    private bool IsTileWithinBounds(Vector2 position)
    {
        return !((position.x < 0 || position.x >= _gridManager.Width) ||
                 (position.y < 0 || position.y >= _gridManager.Height));
    }

    private void HandleRightClick()
    {
        if (DEBUG)
        {
            
        }
    }

    private Vector2 MousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = mousePosition.x;
        float mouseY = mousePosition.y;
        return new Vector2(mouseX, mouseY);
    }
    #endregion
    public void HandleTileClicked(Vector2Int mouseTile)
    {
        Unit u = _levelManager.GetUnitAt(mouseTile);
        //if(u) Debug.Log(u.State);
        switch (_currentState)
        {
            case GameState.PlayerNeutral:
                //Debug.Log($"Tile {mouseTile.x}, {mouseTile.y} Clicked");
                
                //If there isn't a unit at the tile, then do nothing
                if (u == null) return;
                //But if there is, check if it belongs to the active player.
                if (u.PlayerOwner == _activePlayer)
                {
                    //If so, then select it and move on to UnitSelected state.
                    _selectedUnit = u;
                    TransitionState(GameState.UnitSelected);
                }
                else
                {
                    //Else, do nothing for now
                }
                break;
            case GameState.UnitSelected:
                Debug.Log("Unit Selected");
                //!!IMPORTANT!!
                //In final version, the move logic here will be a separate state.
                if (u != null) return;
                
                //Target tile is empty so MOVE OUR GUY OVER THERE
                MovementPath path = MovementUtility.GenerateMovementPath(_gridManager.Grid, _selectedUnit.GetGridPosition(), mouseTile);
                if (path != null && path.Pivots != null && path.Pivots.Count > 0)
                {
                    Debug.Log($"Moving unit from {_selectedUnit.GetGridPosition()} to {mouseTile} with {path.Pivots.Count} pivot points");
                    _movementController.WalkUnit(_selectedUnit, path);
    
                    // Update the unit's grid position after movement
                    _selectedUnit.SetGridPosition(mouseTile);
                }
                else
                {
                    Debug.LogWarning("Cannot generate a valid path to the destination");
                }
                _movementController.WalkUnit(_selectedUnit, path);
                
                break;
            default:
                break;
        }
    }

    private enum GameState
    {
        PlayerNeutral,
        UnitSelected,
        SelectingMoveDestination,
    }
}
