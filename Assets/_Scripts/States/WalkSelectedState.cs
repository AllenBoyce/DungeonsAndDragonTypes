using UnityEngine;

public class WalkSelectedState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void HandleHoverTile(Vector2Int mouseTile)
    {
        Unit selectedUnit = GameManager.Instance.SelectedUnit;
        if(selectedUnit == null) return;
        Unit.Direction direction = MovementUtility.GetDirection(selectedUnit.GetGridPosition(), mouseTile);
        selectedUnit.SetCurrentDirection(direction);
        selectedUnit.PlayAnimation("Idle", direction); //temporary bullshit?
    }

    public override void HandleLeftClickTile(Vector2Int mouseTile)  
    {
        Unit selectedUnit = GameManager.Instance.SelectedUnit;
        if (selectedUnit == null) return;
                
        //Target tile is empty so MOVE OUR GUY OVER THERE
        MovementPath path = MovementUtility.GenerateMovementPath(GameManager.Instance.Grid, selectedUnit.GetGridPosition(), mouseTile);
        if (path != null && path.Pivots != null && path.Pivots.Count > 0)
        {
            //Debug.Log($"Moving unit from {_selectedUnit.GetGridPosition()} to {mouseTile} with {path.Pivots.Count} pivot points");
            GameManager.Instance.MovementController.WalkUnit(selectedUnit, path);
            
        }
        else
        {
            Debug.LogWarning("Cannot generate a valid path to the destination");
        }
        GameManager.Instance.MovementController.WalkUnit(selectedUnit, path);
    }
    public override void HandleRightClickTile(Vector2Int mouseTile)
    {   
        GameManager.Instance.DeselectMove();
    }
}