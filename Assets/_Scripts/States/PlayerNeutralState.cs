using UnityEngine;

public class PlayerNeutralState : GameBaseState
{

    public override void EnterState(GameStateManager gameStateManager)
    {
        
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
        Debug.Log("Blue Flag");
        if(GameManager.Instance.DoesUnitBelongToActivePlayer(GameManager.Instance.GetUnitAt(mouseTile))) {
            Debug.Log("RedFlag");
            GameManager.Instance.SelectUnit(GameManager.Instance.GetUnitAt(mouseTile));
            TransitionToState(GameManager.Instance.GameStateManager.unitSelectedState); //Holy ugly
        }
    }
    public override void HandleRightClickTile(Vector2Int mouseTile)
    {   

    }
}
