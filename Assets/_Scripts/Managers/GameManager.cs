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
    public static event Action<Unit, ScriptableMove> OnMoveSelected;
    public static event Action<Unit, ScriptableMove> OnMoveDeselected;
    public static event Action<Unit, ScriptableMove, Vector2Int> OnUnitAttack;
    public static event Action<Unit, ScriptableMove, Vector2Int> OnUnitHurt; //Unit getting Hurt, Move that damages it, Tile attack originates from
    public static event Action<int> OnActivePlayerChanged;
    public static event Action<int> OnEndGame;
    #endregion
    
    #region Private Variables
    private int _activePlayer;
    private Unit _selectedUnit;
    private ScriptableMove _selectedMove;
    private Vector2Int _hoveredTile;
    private bool _unhandledFaint; //Basically a flag to check if a pokemon has fainted but we haven't handled it yet. This would be used at the end of the state to decide whether we need to transition to checkup state or not.
    private bool _unitLockedIn; //Once a Unit has moved or attacked, it's locked in and can't be deselected until the end of the turn.
    #endregion

    #region Debug Variables
    private bool DEBUG = Constants.DebugMode;
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
            if (Input.GetKeyDown(KeyCode.Escape)) {
                OnEndGame?.Invoke(0);
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

        MovementController.OnUnitMoving += OnUnitMoving;
        MovementController.OnUnitStoppedMoving += OnUnitStoppedMoving;
        if (DEMO)
        {
            Unit u = _unitManager.GenerateUnit(Constants.PokemonSpecies.Garchomp, 0);
            _levelManager.PutUnit(u, 5, 5);
            Unit u2 = _unitManager.GenerateUnit(Constants.PokemonSpecies.Flapple, 1);
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
        //Debug.Log("GameManager TransitionToState complete. Current State: " + _stateManager.CurrentState.ToString());
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
            Debug.Log("GameManager HoverCheck: " + _hoveredTile + " " + mouseTile);
            _hoveredTile = mouseTile;
            OnHoveredTileChanged?.Invoke(_hoveredTile);
        }
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
        //Debug.Log("HandleRightClick: " + mousePosition);
        Vector2Int mouseTile = new Vector2Int(_gridManager.GetGridX(mousePosition.x), _gridManager.GetGridY(mousePosition.y));
        if (IsTileWithinBounds(mouseTile))
        {
            //HandleTileLeftClicked(mouseTile);
            //Debug.Log("HandleRightClick: " + mouseTile);
            OnTileRightClicked?.Invoke(mouseTile);
        }
        else
        {
            //Logic for clicking outside of tile
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
    }

    public Unit GetUnitAt(Vector2Int mouseTile) {
        return _levelManager.GetUnitAt(mouseTile);
    }

    public void SelectUnit(Unit u) {
        //if(_selectedUnit == u) return;
        _selectedUnit = u;
        OnUnitSelected?.Invoke(_selectedUnit);
        TransitionState(_stateManager.unitSelectedState);
    }

    public void DeselectUnit() {
        //Debug.Log("GameManager DeselectUnit");
        _selectedUnit = null;
        OnUnitDeselected?.Invoke(_selectedUnit);
        _uiController.Wipe(); //This should be handled by the UI controller's listener
        TransitionState(_stateManager.playerNeutralState);
    }

    public void DeselectMove() {
        _selectedMove = null;
        ClearPathPreview();
        TransitionState(_stateManager.unitSelectedState);
    }

    public int CalculateDamage(Unit attacker, ScriptableMove move, Unit target) {
        return move.power;
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
            TransitionState(GameStateManager.walkSelectedState);
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

        TransitionState(GameStateManager.moveSelectedState);
        OnMoveSelected?.Invoke(u, move);
        
    }

    public List<Tile> GetTargetedTiles(ScriptableMove move) {
        Debug.Log("GameManager GetTargetedTiles: " + _hoveredTile + " " + _selectedUnit + " " + move);
        return TargetingUtility.GetTiles(_gridManager.Grid, _hoveredTile, _selectedUnit, move);
    }

    // TO BE USED IN EXECUTE MOVE STATE
    public List<Unit> GetTargetedUnits(ScriptableMove move) {
        List<Tile> targetedTiles = GetTargetedTiles(move);
        List<Unit> targetedUnits = new List<Unit>();
        foreach (Tile tile in targetedTiles)
        {
            Unit target = _levelManager.GetUnitAt(new Vector2Int(tile.x, tile.y));
            if (target == null) continue;
            targetedUnits.Add(target);
        }

        return targetedUnits;
    }

    public void AlertHurtUnits(List<Unit> hurtUnits, ScriptableMove move, Vector2Int originTile) {
        foreach (Unit unit in hurtUnits) {
            OnUnitHurt?.Invoke(unit, move, originTile);
        }
    }
    

    public void OnUnitMoving(Unit u, MovementPath path) {
        Debug.Log("GameManager OnUnitMoving: " + u.name + " with path: " + path.Pivots.Count);
        u.UpdateState(Unit.UnitState.Moving);
    }

    public void OnUnitStoppedMoving(Unit u, MovementPath path) {
        Debug.Log("GameManager OnUnitStoppedMoving: " + u.name + " with path: " + path.Pivots.Count);
        u.UpdateState(Unit.UnitState.Idle);
        ClearPathPreview();
        TransitionState(_stateManager.unitSelectedState);
    }
    
    public List<Unit> Units { get { return _levelManager.Units; } }

    public void HandleAttack(Unit attacker, ScriptableMove move, Vector2Int mouseTile)
    {
        //Ignoring Validation for now
        OnUnitAttack?.Invoke(attacker, move, mouseTile);

        TransitionState(_stateManager.executeMoveState);
    }

    public void HandleHurt(Unit attacker, ScriptableMove move, Unit target) {
        int damage = CalculateDamage(attacker, move, target);
        target.Hurt(damage);
    }

    public void SwitchPlayer() {
        _activePlayer = 1 - _activePlayer;
        _selectedUnit = null;
        _selectedMove = null;
        OnActivePlayerChanged?.Invoke(_activePlayer);
        TransitionState(_stateManager.playerNeutralState);
    }

    public void UpdatePathPreview(MovementPath path) {
        ClearPathPreview();
        foreach(Vector2Int tile in path.Pivots) {
            Tile t = _gridManager.Grid[tile];
            t.SetPathPreview(true);
        }
    }

    public void SetPathPreview(MovementPath path) {
        ClearPathPreview();
        if (path == null || path.Pivots.Count < 2) return;

        // Iterate through each segment of the path
        for (int i = 0; i < path.Pivots.Count - 1; i++) {
            Vector2Int start = path.Pivots[i];
            Vector2Int end = path.Pivots[i + 1];

            // Get all tiles between start and end points
            List<Vector2Int> lineTiles = GetLineTiles(start, end);
            foreach (Vector2Int tile in lineTiles) {
                if (_gridManager.Grid.TryGetValue(tile, out Tile t)) {
                    t.SetPathPreview(true);
                }
            }
        }
    }

    private List<Vector2Int> GetLineTiles(Vector2Int start, Vector2Int end) {
        List<Vector2Int> lineTiles = new List<Vector2Int>();
        int dx = Mathf.Abs(end.x - start.x);
        int dy = Mathf.Abs(end.y - start.y);
        int sx = start.x < end.x ? 1 : -1;
        int sy = start.y < end.y ? 1 : -1;
        int err = dx - dy;

        while (true) {
            lineTiles.Add(new Vector2Int(start.x, start.y));
            if (start.x == end.x && start.y == end.y) break;
            int e2 = 2 * err;
            if (e2 > -dy) {
                err -= dy;
                start.x += sx;
            }
            if (e2 < dx) {
                err += dx;
                start.y += sy;
            }
        }
        return lineTiles;
    }

    public void ClearPathPreview() {
        foreach(Tile t in _gridManager.Grid.Values) {
            t.SetPathPreview(false);
        }
    }

    public void CheckEndGame() {
        if(IsGameOver()) {
            OnEndGame?.Invoke(GetWinningPlayer());
        }
    }

    private bool IsGameOver() {
        if(GetWinningPlayer() != -1) return true;
        return false;
    }
    private int GetWinningPlayer() {
        bool playerOneTeamAlive = false;
        bool playerTwoTeamAlive = false;
        foreach(Unit u in _levelManager.Units) {
            if(u.State == Unit.UnitState.Fainted) continue;
            if(u.PlayerOwner == 0) playerOneTeamAlive = true;
            else playerTwoTeamAlive = true;
        }
        if(!playerOneTeamAlive) return 1;
        if(!playerTwoTeamAlive) return 0;
        return -1;
    }

    #region Getters and Setters
    public Unit SelectedUnit { get { return _selectedUnit; } }
    public ScriptableMove SelectedMove { get { return _selectedMove; } }
    public Vector2Int HoveredTile { get { return _hoveredTile; } }
    public GameBaseState CurrentState { get {return _stateManager.CurrentState;} }
    public Dictionary<Vector2Int, Tile> Grid { get { return _gridManager.Grid; } }
    public GameStateManager GameStateManager { get { return _stateManager; } }
    public MovementController MovementController { get { return _movementController; } }
    public bool UnhandledFaint { get { return _unhandledFaint; } set { _unhandledFaint = value; } }
    public bool UnitLockedIn { get { return _unitLockedIn; } set { _unitLockedIn = value; } }
    public (int, int) GetCurrentAP() {
        if(_selectedUnit == null) throw new Exception("No unit selected");
        return (_selectedUnit.CurrentAP, _selectedUnit.MaxAP);
    }

    public int ActivePlayer { get { return _activePlayer; } }
    #endregion

    #region Utility Methods for States
    public bool DoesUnitBelongToActivePlayer(Unit u) {
        //Debug.Log("DoesUnitBelongToActivePlayer: " + u.PlayerOwner + " == " + _activePlayer);
        return u.PlayerOwner == _activePlayer;
    }
    #endregion

}
