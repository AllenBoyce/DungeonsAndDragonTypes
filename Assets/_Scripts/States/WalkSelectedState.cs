using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class WalkSelectedState : GameBaseState
{
    private GameStateManager _gameStateManager;
    public override void EnterState(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void HandleHoverTile(Vector2Int mouseTile)
    {
        Unit selectedUnit = GameManager.Instance.SelectedUnit;
        if(selectedUnit == null || selectedUnit.State == Unit.UnitState.Moving) return;
        
        Unit.Direction direction = MovementUtility.GetDirection(selectedUnit.GetGridPosition(), mouseTile);
        selectedUnit.SetCurrentDirection(direction);
        selectedUnit.PlayAnimation("Idle", direction); //temporary bullshit?

        MovementPath path = MovementUtility.GenerateMovementPath(GameManager.Instance.Grid, selectedUnit.GetGridPosition(), mouseTile);
        int distance = path.Distance();
        int APCost =  CalculateAPCost(distance, selectedUnit.PokemonData.BaseStats.moveSpeed);
        
            int maxDistance = selectedUnit.CurrentAP * selectedUnit.PokemonData.BaseStats.moveSpeed;
            MovementPath truncatedPath = MovementUtility.TruncatePath(path, maxDistance);
            Debug.Log("Max Distance: " + maxDistance);
            Debug.Log("Truncated path: " + truncatedPath.Pivots.Count);
            Debug.Log("AP Cost: " + APCost);
            path = truncatedPath;
        
        UIController.Instance.DisplayTempAP(APCost);

        GameManager.Instance.SetPathPreview(path);
        //Debug.Log("WalkSelectedState HandleHoverTile: " + path.Pivots.Count);
    }

    private int CalculateAPCost(int distance, int moveSpeed) {
        int APCost =  (distance - 1)/moveSpeed + 1;
        return APCost;
    }

    public override async void HandleLeftClickTile(Vector2Int mouseTile)  //TODO: Later we'll put this in it's own state
    {
        Unit selectedUnit = GameManager.Instance.SelectedUnit;
        if (selectedUnit == null) return;
                
        //Target tile is empty so MOVE OUR GUY OVER THERE
        
        MovementPath path = MovementUtility.GenerateMovementPath(GameManager.Instance.Grid, selectedUnit.GetGridPosition(), mouseTile);
        int maxDistance = selectedUnit.CurrentAP * selectedUnit.PokemonData.BaseStats.moveSpeed;
        path = MovementUtility.TruncatePath(path, maxDistance);
        int cost = CalculateAPCost(path.Distance(), selectedUnit.PokemonData.BaseStats.moveSpeed);
        if(cost > selectedUnit.CurrentAP) {
            Debug.LogWarning("Not enough AP to move");
            return;
        }
        selectedUnit.ConsumeAP(cost);
        selectedUnit.UpdateState(Unit.UnitState.Moving);
        if (path != null && path.Pivots != null && path.Pivots.Count > 0)
        {
            //Ignore warning
            GameManager.Instance.MovementController.WalkUnit(selectedUnit, path);
            
        }
        else
        {
            Debug.LogWarning("Cannot generate a valid path to the destination");
        }
        await GameManager.Instance.MovementController.WalkUnit(selectedUnit, path);
        selectedUnit.UpdateState(Unit.UnitState.Idle);
        if(selectedUnit.CurrentAP <= 0) {
            GameManager.Instance.TransitionState(_gameStateManager.checkupState);
        }
        else {
            GameManager.Instance.TransitionState(_gameStateManager.unitSelectedState);
        }
        UIController.Instance.ClearMovementPath();
    }
    public override void HandleRightClickTile(Vector2Int mouseTile)
    {   
        GameManager.Instance.DeselectMove();
    }
}