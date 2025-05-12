using UnityEngine;
using System.Collections.Generic;

public class CheckupState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        
    }

    private void Continue(GameStateManager gameStateManager)
    {
        if(GameManager.Instance.SelectedUnit.CurrentAP > 0) {
            Debug.Log("CheckupState: Transitioning to unitSelectedState");
            Debug.Log("CheckupState: SelectedUnit: " + GameManager.Instance.SelectedUnit.name);
            GameManager.Instance.TransitionState(gameStateManager.unitSelectedState);
        }
        else {
            Debug.Log("CheckupState: Switching player");
            GameManager.Instance.SwitchPlayer();
            GameManager.Instance.TransitionState(gameStateManager.playerNeutralState);
        }
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        if(GameManager.Instance.IsGameOver()) {
            GameManager.Instance.CheckEndGame();
        }
        else {
            Continue(gameStateManager);
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