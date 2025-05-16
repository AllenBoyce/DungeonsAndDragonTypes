using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This state is used to execute a move. It is called when the player has selected a valid move and a target tile pair.
/// It will handle the animation, the damage, and the loss of AP.
/// After the animation, it will switch to the MoveCheckupState.
/// </summary>
public class ExecuteMoveState : GameBaseState
{
    private Unit _attackingUnit;
    private List<Unit> _defendingUnits;
    private List<Tile> _affectedTiles;
    private ScriptableMove _move;
    private Vector2Int _targetedTile;
    private bool _shouldTransition = false;
    private GameBaseState _nextState;

    public override async void EnterState(GameStateManager gameStateManager)
    {
        GameManager.Instance.UnitLockedIn = true;

        _attackingUnit = GameManager.Instance.SelectedUnit;
        _move = GameManager.Instance.SelectedMove;
        _targetedTile = GameManager.Instance.TargetedTile;

        _affectedTiles = GameManager.Instance.GetTargetedTiles(_move, _targetedTile);
        _defendingUnits = GameManager.Instance.GetTargetedUnits(_move, _targetedTile);
        _defendingUnits.RemoveAll(unit => unit.State == Unit.UnitState.Fainted);
        _defendingUnits.Remove(_attackingUnit); //I think this works
        
        GameManager.Instance.PlaySFX(_move.sfx);

        //Handle AP Changes
        _attackingUnit.ConsumeAP(_move.apCost);
        Debug.Log("GameManager HandleAttack: " + _attackingUnit.CurrentAP);

        await _attackingUnit.PlayAnimationAsync(_move.animationKey, _attackingUnit.GetCurrentDirection());
        _attackingUnit.PlayAnimation("Idle", _attackingUnit.GetCurrentDirection());
        
        if(_defendingUnits.Count > 0) {
            GameManager.Instance.TransitionState(gameStateManager.impactTargetState);
        }
        else {
            GameManager.Instance.TransitionState(gameStateManager.checkupState);
        }
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