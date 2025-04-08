using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameBaseState CurrentState {get; private set;}
    public PlayerNeutralState playerNeutralState = new();
    public UnitSelectedState unitSelectedState = new();
    public WalkSelectedState walkSelectedState = new();
    public MoveSelectedState moveSelectedState = new();
    public ExecuteMoveState executeMoveState = new();

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

    void Start()
    {
        GameManager.OnHoveredTileChanged += OnHoveredTileChanged;
        GameManager.OnTileLeftClicked += OnTileLeftClicked;


        CurrentState = playerNeutralState;
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
