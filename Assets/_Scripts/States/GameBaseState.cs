//https://www.youtube.com/watch?v=Vt8aZDPzRjI
using UnityEngine;

public abstract class GameBaseState
{

    public abstract void EnterState(GameStateManager gameStateManager);
    
    /// <summary>
    /// Called every frame while the state is active
    /// </summary>
    /// <param name="gameStateManager">The GameStateManager instance</param>
    public abstract void UpdateState(GameStateManager gameStateManager);

    /// <summary>
    /// Called whenever the mouse hovers over a new tile
    /// Called by GameStateManager's OnHoveredTileChanged event
    /// </summary>
    /// <param name="mouseTile">The tile that the mouse is hovering over</param>
    public abstract void HandleHoverTile(Vector2Int mouseTile);

    public abstract void HandleLeftClickTile(Vector2Int mouseTile);

    public abstract void HandleRightClickTile(Vector2Int mouseTile);

}
