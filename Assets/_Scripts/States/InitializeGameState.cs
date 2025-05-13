using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class InitializeGameState : GameBaseState
{
    public override async void EnterState(GameStateManager gameStateManager)
    {
        // TODO: Spawn the units
        // Animations and cries
        foreach (Unit unit in GameManager.Instance.Units) {
            GameManager.Instance.StartCoroutine(Pause(1f));
            AudioController.Instance.PlaySound(unit.PokemonData.cry);
            unit.SetCurrentDirection(Unit.Direction.South);
            await unit.PlayAnimationAsync("Charge");
            unit.PlayAnimation("Idle");
            //GameManager.Instance.StartCoroutine(Pause(1f));
        }
        // Decide who goes first
        // TODO: Move to playerneutral state
        GameManager.Instance.StartGame();
    }

    IEnumerator Pause(float seconds)
    {
        yield return new WaitForSeconds(seconds);
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
