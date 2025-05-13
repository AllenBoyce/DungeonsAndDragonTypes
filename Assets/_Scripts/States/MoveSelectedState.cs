using UnityEngine;

public class MoveSelectedState : GameBaseState
{

    public override void EnterState(GameStateManager gameStateManager)
    {
        ScriptableMove move = GameManager.Instance.SelectedMove;
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
        ScriptableMove move = GameManager.Instance.SelectedMove;
        if(selectedUnit == null) return;
        
        if(move.apCost > selectedUnit.CurrentAP) {
            AudioController.Instance.PlaySound(Resources.Load<AudioClip>("Audio/SFX/MiscSFX/UIDeny"));
            return;
        }

        GameManager.Instance.HandleAttack(selectedUnit, move, mouseTile);

    }
    public override void HandleRightClickTile(Vector2Int mouseTile)
    {   
        GameManager.Instance.DeselectMove();
    }
}