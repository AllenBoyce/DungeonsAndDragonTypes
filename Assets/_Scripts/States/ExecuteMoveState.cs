using System.Collections.Generic;
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
    private Vector2Int _originTile;
    private bool _shouldTransition = false;
    private GameBaseState _nextState;

    public override void EnterState(GameStateManager gameStateManager)
    {
        GameManager.Instance.UnitLockedIn = true;

        _attackingUnit = GameManager.Instance.SelectedUnit;
        _move = GameManager.Instance.SelectedMove;
        _originTile = GameManager.Instance.HoveredTile;

        _affectedTiles = GameManager.Instance.GetTargetedTiles(_move);
        _defendingUnits = GameManager.Instance.GetTargetedUnits(_move);
        _defendingUnits.RemoveAll(unit => unit.State == Unit.UnitState.Fainted);
        _defendingUnits.Remove(_attackingUnit); //I think this works
        
        GameManager.Instance.AlertHurtUnits(_defendingUnits, _move, _originTile);

        //Handle AP Changes
        _attackingUnit.ConsumeAP(_move.apCost);
        Debug.Log("GameManager HandleAttack: " + _attackingUnit.CurrentAP);
        
        // Set up the next state transition but don't execute it immediately 
        if(_attackingUnit.CurrentAP <= 0) { //No more AP
            _shouldTransition = true;
            _nextState = gameStateManager.playerNeutralState;
            Debug.Log("GameManager HandleAttack: Will switch player next frame");
        } else { //AP remains, go again
            _shouldTransition = true;
            _nextState = gameStateManager.unitSelectedState;
            Debug.Log("GameManager HandleAttack: Will transition to UnitSelectedState next frame");
        }
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        // Handle the actual state transition in the next frame
        if (_shouldTransition)
        {
            _shouldTransition = false;
            
            if (_nextState == gameStateManager.playerNeutralState)
            {
                GameManager.Instance.SwitchPlayer();
            }
            else
            {
                GameManager.Instance.TransitionState(_nextState);
            }
        }
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