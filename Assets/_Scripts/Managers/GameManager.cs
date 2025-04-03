using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //private GameBaseState _currentState;
    private GameState _currentState;
    private UnitManager _unitManager;
    private LevelManager _levelManager;
    private GridManager _gridManager;
    private MovementController _movementController;
    private UIController _uiController;
    
    //Variables pertaining to the game
    private int _activePlayer;
    private Unit _selectedUnit;
    private ScriptableMove _selectedMove;

    private bool DEBUG = true;
    private bool DEMO = true;
    void Start()
    {
        _unitManager = FindFirstObjectByType<UnitManager>();
        _levelManager = FindFirstObjectByType<LevelManager>();
        _gridManager = FindFirstObjectByType<GridManager>();
        _movementController = FindFirstObjectByType<MovementController>();
        _uiController = FindFirstObjectByType<UIController>();


        if (DEMO)
        {
            Unit u = _unitManager.GenerateUnit("Garchomp", 0);
            _levelManager.PutUnit(u, 5, 5);
            _selectedUnit = u;
            Unit u2 = _unitManager.GenerateUnit("Flapple", 0);
            _levelManager.PutUnit(u2, 8, 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ClickCheck();
        HoverCheck();
    }

    private void TransitionState(GameState nextState)
    {
        Debug.Log("Transitioning State to: " + nextState);
        
        //TEMPORARY LOGIC MOVE LATER
        if(_currentState != GameState.UnitSelected && nextState == GameState.UnitSelected) _uiController.DisplayUnitControls(_selectedUnit);
        
        //TEMPORARY LOGIC MOVE LATER
        //if(nextState == GameState.MoveSelected)

        if (_currentState == GameState.UnitAttacking && nextState == GameState.PlayerNeutral)
        {
            Debug.Log("Clearing Attacking Unit");
            _selectedUnit = null;
            _uiController.Wipe();
        }
        
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

    private void HoverCheck()
    {
        Vector2 mousePosition = MousePosition();
        Vector2Int mouseTile = new Vector2Int(_gridManager.GetGridX(mousePosition.x), _gridManager.GetGridY(mousePosition.y));
        
        switch(_currentState)
        {
            case GameState.MoveSelected:
                
                Unit.Direction direction = MovementUtility.GetDirection(_selectedUnit.GetGridPosition(), mouseTile);
                _selectedUnit.SetCurrentDirection(direction);
                _selectedUnit.PlayAnimation("Idle", direction); //temporary bullshit
                _uiController.HighlightTargetedTiles(_selectedMove, mouseTile, _selectedUnit.GetCurrentDirection(), _gridManager.Grid);
                break;
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

        if (_currentState == GameState.MoveSelected)
        {
            _selectedMove = null;
            _uiController.ClearHighlightedTiles(_gridManager.Grid);
            TransitionState(GameState.UnitSelected);
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
                    //_uiController.DisplayUnitControls(_selectedUnit);
                    TransitionState(GameState.UnitSelected);
                }
                else
                {
                    //Else, do nothing for now
                }
                break;
            case GameState.UnitSelected:
                
                break;
            case GameState.WalkSelected:
                //!!IMPORTANT!!
                //In final version, the move logic here will be a separate state.
                if (u != null) return;
                
                //Target tile is empty so MOVE OUR GUY OVER THERE
                MovementPath path = MovementUtility.GenerateMovementPath(_gridManager.Grid, _selectedUnit.GetGridPosition(), mouseTile);
                if (path != null && path.Pivots != null && path.Pivots.Count > 0)
                {
                    //Debug.Log($"Moving unit from {_selectedUnit.GetGridPosition()} to {mouseTile} with {path.Pivots.Count} pivot points");
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
            case GameState.MoveSelected:
                TransitionState(GameState.UnitAttacking);
                HandleAttack(_selectedUnit, _selectedMove, mouseTile);
                TransitionState(GameState.PlayerNeutral);
                break;
            default:
                break;
        }
    }

    public void EndTurn()
    {
        _uiController.Wipe();
        TransitionState(GameState.PlayerNeutral);
    }

    public void SelectMove(Unit u, string moveName)
    {
        if (u != _selectedUnit)
        {
            Debug.LogWarning("Selected unit does not match the currently selected unit");
            return;
        }

        if (moveName == "Move")
        {
            TransitionState(GameState.WalkSelected);
            return;
        }

        Dictionary<string, ScriptableMove> moveDict = u.GetMoveDictionary();
        if (!moveDict.ContainsKey(moveName))
        {
            Debug.LogWarning("Selected move does not match the currently selected unit");
            return;
        }
        
        ScriptableMove move = moveDict[moveName];
        _selectedMove = move;
        TransitionState(GameState.MoveSelected);
        
    }

    private void HandleAttack(Unit attacker, ScriptableMove move, Vector2Int mouseTile)
    {
        //Ignoring Validation for now
        attacker.PlayAnimation(move.animationKey, attacker.GetCurrentDirection(), false);
        attacker.PlayAnimation("Idle", attacker.GetCurrentDirection());
        List<Tile> targetedTiles =
            TargetingUtility.GetTiles(_gridManager.Grid, mouseTile, attacker.GetCurrentDirection(), move);
        Debug.Log(targetedTiles.Count);
        _uiController.ClearHighlightedTiles(_gridManager.Grid);
        foreach (Tile tile in targetedTiles)
        {
            Unit target = _levelManager.GetUnitAt(new Vector2Int(tile.x, tile.y));
            if (target == null) continue;
            target.Hurt(move.power);
        }

    }

    private enum GameState
    {
        PlayerNeutral,
        UnitSelected,
        WalkSelected,
        MoveSelected,
        UnitAttacking,
        UnitWalking,
    }
}
