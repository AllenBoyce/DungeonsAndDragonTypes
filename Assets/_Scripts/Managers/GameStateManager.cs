using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameBaseState CurrentState {get; private set;}
    public PlayerNeutralState playerNeutralState = new PlayerNeutralState();
    public UnitSelectedState unitSelectedState = new UnitSelectedState();
    public WalkSelectedState walkSelectedState = new WalkSelectedState();
    public MoveSelectedState moveSelectedState = new MoveSelectedState();
    public ExecuteMoveState executeMoveState = new ExecuteMoveState();
    public InitializeGameState initializeGameState = new InitializeGameState();
    public ImpactTargetState impactTargetState = new ImpactTargetState();
    public CheckupState checkupState = new CheckupState();
    public GameOverState gameOverState = new GameOverState();

    /// <summary>
    /// Invokes the CurrentState's HandleHoverTile method whenever the mouse hovers over a new tile
    /// </summary>
    /// <param name="mouseTile">The tile that the mouse is hovering over</param>
    public void OnHoveredTileChanged(Vector2Int mouseTile) {
        CurrentState.HandleHoverTile(mouseTile);
    }

    /// <summary>
    /// Invokes the CurrentState's HandleLeftClickTile method whenever the left mouse button is clicked
    /// </summary>
    /// <param name="mouseTile">The tile that the mouse is clicked on</param>
    public void OnTileLeftClicked(Vector2Int mouseTile) {
        CurrentState.HandleLeftClickTile(mouseTile);
    }

    /// <summary>
    /// Invokes the CurrentState's HandleRightClickTile method whenever the right mouse button is clicked
    /// </summary>
    /// <param name="mouseTile">The tile that the mouse is clicked on</param>
    public void OnTileRightClicked(Vector2Int mouseTile) {
        //Debug.Log("GameStateManager: OnTileRightClicked: " + mouseTile);
        //Debug.Log("CurrentState: " + CurrentState.ToString());
        CurrentState.HandleRightClickTile(mouseTile);
    }

    void Start()
    {
        GameManager.OnHoveredTileChanged += OnHoveredTileChanged;
        GameManager.OnTileLeftClicked += OnTileLeftClicked;
        GameManager.OnTileRightClicked += OnTileRightClicked;

        CurrentState = initializeGameState;
        CurrentState.EnterState(this);
    }

    void Update()
    {
        CurrentState.UpdateState(this);
    }

    public void TransitionToState(GameBaseState newState) {
        Debug.Log("Transitioning to state: " + newState.GetType().Name);
        CurrentState = newState;
        CurrentState.EnterState(this);
    }
}
