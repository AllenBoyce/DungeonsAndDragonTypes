using UnityEngine;
using System.Collections.Generic;

public class MoveCheckupState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        // List<Unit> units = GameManager.Instance.Units;
        // List<Unit> faintedUnits = units.Where(u => u.State == Unit.UnitState.Fainted).ToList();
        // if(faintedUnits.Count == units.Count) {
        //     gameStateManager.SwitchState(gameStateManager.GameOverState);
        // }
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

    }
        
}