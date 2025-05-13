using UnityEngine;

public class GameOverState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        AudioController.Instance.PlaySFX(Resources.Load<AudioClip>("Audio/SFX/MiscSFX/Victory"));
        AudioController.Instance.StopMusic();
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
    
    
    