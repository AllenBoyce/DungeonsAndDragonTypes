using UnityEngine;

public class PlayerNeutralState : GameBaseState
{

    public override void EnterState(GameStateManager gameStateManager)
    {
        GameManager.Instance.UnitLockedIn = false;
    }

    /// <summary>
    /// Called every frame while the state is active
    /// </summary>
    /// <param name="gameStateManager">The GameStateManager instance</param>
    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void HandleHoverTile(Vector2Int mouseTile)
    {

    }

    public override void HandleLeftClickTile(Vector2Int mouseTile)  
    {
        Unit u = GameManager.Instance.GetUnitAt(mouseTile);
        if(u == null) return;
        if(GameManager.Instance.DoesUnitBelongToActivePlayer(u) && u.State != Unit.UnitState.Fainted) {
            GameManager.Instance.SelectUnit(u);
            return;
        }
    }
    public override void HandleRightClickTile(Vector2Int mouseTile)
    {   

    }
}
