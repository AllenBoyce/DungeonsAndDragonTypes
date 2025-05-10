using UnityEngine;

public class UnitSelectedState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void HandleHoverTile(Vector2Int mouseTile)
    {

    }

    public override void HandleLeftClickTile(Vector2Int mouseTile)  
    {

    }
    public override void HandleRightClickTile(Vector2Int mouseTile)
    {   
        //Debug.Log("UnitSelectedState: HandleRightClickTile");
        //Deselect this unit
        if(!GameManager.Instance.UnitLockedIn) GameManager.Instance.DeselectUnit();
    }
}
