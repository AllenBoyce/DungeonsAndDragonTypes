using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake() {
        Instance = this;
    }

    #region Managers
    //private GameState _currentState;
    private UnitManager _unitManager;
    private LevelManager _levelManager;
    private GridManager _gridManager;
    private MovementController _movementController;
    private UIController _uiController;

    private GameStateManager _stateManager;
    // private GameBaseState _currentGameState;
    #endregion
    
    /*
    Events for:
    - Game State Change
        Listeners: UIController
    - Hovered Tile Change
        Listeners: 
    - Unit Selected
        Listeners:
    - Unit Deselected
        Listeners:
    - Move Selected
        Listeners:
    - Unit Attacks
        Listeners: AnimationController
    - Unit Takes Damage
        Listeners: AnimationController
    - Unit Dies
        Listeners: AnimationController
    - Unit Moves
        Listeners:
    - Active Player Change
        Listeners:
    */
    #region Events
    public static event Action<GameBaseState> OnGameStateChanged;
    public static event Action<Vector2Int> OnHoveredTileChanged;
    public static event Action<Vector2Int> OnTileLeftClicked;
    public static event Action<Vector2Int> OnTileRightClicked;
    public static event Action<Unit> OnUnitSelected;
    public static event Action<Unit> OnUnitDeselected;
    public static event Action<Unit, ScriptableMove, Vector2Int> OnUnitAttack;
    public static event Action<Unit, ScriptableMove, Vector2Int> OnUnitHurt; //Unit getting Hurt, Move that damages it, Tile attack originates from
    #endregion
    
    #region Private Variables
    private int _activePlayer;
    private Unit _selectedUnit;
    private ScriptableMove _selectedMove;
    private Vector2Int _hoveredTile;
    #endregion

    #region Debug Variables
    private bool DEBUG = true;
    private bool DEMO = true;
    #endregion

    #region Debug Methods
    private void DebugLog(string message) {
        if(DEBUG) Debug.Log(message);
    }
    private void DebugCheck() {
        if (DEBUG) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("Current State: " + _stateManager.CurrentState.ToString());
                Debug.Log("Current State: " + CurrentState.ToString());
                Debug.Log("Selected Unit: " + _selectedUnit);
            }
        }
    }

    #endregion

    void Start()
    {
        _unitManager = FindFirstObjectByType<UnitManager>();
        _levelManager = FindFirstObjectByType<LevelManager>();
        _gridManager = FindFirstObjectByType<GridManager>();
        _movementController = FindFirstObjectByType<MovementController>();
        _uiController = FindFirstObjectByType<UIController>();
        _stateManager = GetComponent<GameStateManager>();

        
        if (DEMO)
        {
            Unit u = _unitManager.GenerateUnit(Constants.PokemonSpecies.Garchomp, 0);
            _levelManager.PutUnit(u, 5, 5);
            Unit u2 = _unitManager.GenerateUnit(Constants.PokemonSpecies.Flapple, 0);
            _levelManager.PutUnit(u2, 8, 5);

            _uiController.Initialize(); //SLOPPY AND TEMPORARY
        }
    }

    // Update is called once per frame
    void Update()
    {
        DebugCheck();
        ClickCheck();
        HoverCheck();
    }

    
    /// <summary>
    /// Transitions to a new state by calling the GameStateManager's TransitionToState method and invoking the OnGameStateChanged event.
    /// </summary>
    /// <param name="nextState">The new state to transition to.</param>
    public void TransitionState(GameBaseState nextState)
    {
        Debug.Log("GameManager calling TransitionToState " + nextState.ToString());
        _stateManager.TransitionToState(nextState);
        Debug.Log("GameManager TransitionToState complete. Current State: " + _stateManager.CurrentState.ToString());
        OnGameStateChanged?.Invoke(nextState);
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
            HandleRightClick(MousePosition());
        }
    }
    private void HoverCheck()
    {
        Vector2 mousePosition = MousePosition();
        Vector2Int mouseTile = new Vector2Int(_gridManager.GetGridX(mousePosition.x), _gridManager.GetGridY(mousePosition.y));
        if(_hoveredTile != mouseTile) {
            _hoveredTile = mouseTile;
            OnHoveredTileChanged?.Invoke(_hoveredTile);
        }

        //     case GameState.WalkSelected:
        //         //Generate path
        //         MovementPath path = MovementUtility.GenerateMovementPath(_gridManager.Grid, _selectedUnit.GetGridPosition(), mouseTile);
        //         _uiController.PreviewMovementPath(path);
        //         break;
        //     case GameState.MoveSelected:
                
        //         Unit.Direction direction = MovementUtility.GetDirection(_selectedUnit.GetGridPosition(), mouseTile);
        //         _selectedUnit.SetCurrentDirection(direction);
        //         _selectedUnit.PlayAnimation("Idle", direction); //temporary bullshit
        //         _uiController.HighlightTargetedTiles(_selectedMove, mouseTile, _selectedUnit.GetCurrentDirection(), _gridManager.Grid);
        //         break;
        // }
    }

    private void HandleLeftClick(Vector2 mousePosition)
    {
        Vector2Int mouseTile = new Vector2Int(_gridManager.GetGridX(mousePosition.x), _gridManager.GetGridY(mousePosition.y));
        if (IsTileWithinBounds(mouseTile))
        {
            //HandleTileLeftClicked(mouseTile);
            OnTileLeftClicked?.Invoke(mouseTile);
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

    private void HandleRightClick(Vector2 mousePosition)
    {
        if (DEBUG)
        {
            
        }
        Debug.Log("HandleRightClick: " + mousePosition);
        Vector2Int mouseTile = new Vector2Int(_gridManager.GetGridX(mousePosition.x), _gridManager.GetGridY(mousePosition.y));
        if (IsTileWithinBounds(mouseTile))
        {
            //HandleTileLeftClicked(mouseTile);
            Debug.Log("HandleRightClick: " + mouseTile);
            OnTileRightClicked?.Invoke(mouseTile);
        }
        else
        {
            //Logic for clicking outside of tile
        }

        // if (_currentState == GameState.MoveSelected)
        // {
        //     _selectedMove = null;
        //     _uiController.ClearHighlightedTiles(_gridManager.Grid);
        //     TransitionState(GameState.UnitSelected);
        // }
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
        //     case GameState.UnitSelected:
        //         if(u == null) return;
        //         if(u == _selectedUnit) DeselectUnit();
        //         break;
        //     case GameState.WalkSelected:
        //         //!!IMPORTANT!!
        //         //In final version, the move logic here will be a separate state.
        //         if (u != null) return;
                
        //         //Target tile is empty so MOVE OUR GUY OVER THERE
        //         MovementPath path = MovementUtility.GenerateMovementPath(_gridManager.Grid, _selectedUnit.GetGridPosition(), mouseTile);
        //         if (path != null && path.Pivots != null && path.Pivots.Count > 0)
        //         {
        //             //Debug.Log($"Moving unit from {_selectedUnit.GetGridPosition()} to {mouseTile} with {path.Pivots.Count} pivot points");
        //             _movementController.WalkUnit(_selectedUnit, path);
    
        //             // Update the unit's grid position after movement
        //             _selectedUnit.SetGridPosition(mouseTile);
        //         }
        //         else
        //         {
        //             Debug.LogWarning("Cannot generate a valid path to the destination");
        //         }
        //         _movementController.WalkUnit(_selectedUnit, path);
        //         break;
        //     case GameState.MoveSelected: //THIS has GOTTA change.
        //         TransitionState(GameState.UnitAttacking);
        //         HandleAttack(_selectedUnit, _selectedMove, mouseTile);
        //         TransitionState(GameState.PlayerNeutral);
        //         break;
        //     default:
        //         break;
        // }
    }

    public Unit GetUnitAt(Vector2Int mouseTile) {
        return _levelManager.GetUnitAt(mouseTile);
    }

    public void SelectUnit(Unit u) {
        if(_selectedUnit == u) return;
        _selectedUnit = u;
        OnUnitSelected?.Invoke(_selectedUnit);
        TransitionState(_stateManager.unitSelectedState);
    }

    public void DeselectUnit() {
        Debug.Log("GameManager DeselectUnit");
        _selectedUnit = null;
        OnUnitDeselected?.Invoke(_selectedUnit);
        _uiController.Wipe(); //This should be handled by the UI controller's listener
        TransitionState(_stateManager.playerNeutralState);
    }

    public void EndTurn()
    {
        _uiController.Wipe();
        //TransitionState(GameState.PlayerNeutral);
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
            //TransitionState(GameState.WalkSelected);
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
       // TransitionState(GameState.MoveSelected);
        
    }
    
    public List<Unit> Units { get { return _levelManager.Units; } }

    private void HandleAttack(Unit attacker, ScriptableMove move, Vector2Int mouseTile)
    {
        //Ignoring Validation for now
        OnUnitAttack?.Invoke(attacker, move, mouseTile);
        List<Tile> targetedTiles =
            TargetingUtility.GetTiles(_gridManager.Grid, mouseTile, attacker.GetCurrentDirection(), move);
        foreach (Tile tile in targetedTiles)
        {
            Unit target = _levelManager.GetUnitAt(new Vector2Int(tile.x, tile.y));
            if (target == null) continue;
            OnUnitHurt?.Invoke(target, move, mouseTile);
        }

    }

    #region Getters and Setters
    public Unit SelectedUnit { get { return _selectedUnit; } }
    public ScriptableMove SelectedMove { get { return _selectedMove; } }
    public Vector2Int HoveredTile { get { return _hoveredTile; } }
    public GameBaseState CurrentState { get {return _stateManager.CurrentState;} }
    public Dictionary<Vector2Int, Tile> Grid { get { return _gridManager.Grid; } }
    public GameStateManager GameStateManager { get { return _stateManager; } }
    #endregion

    #region Utility Methods for States
    public bool DoesUnitBelongToActivePlayer(Unit u) {
        Debug.Log("DoesUnitBelongToActivePlayer: " + u.PlayerOwner + " == " + _activePlayer);
        return u.PlayerOwner == _activePlayer;
    }
    #endregion
}
