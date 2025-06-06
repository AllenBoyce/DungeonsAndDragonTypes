using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ImpactTargetState : GameBaseState
{
    private Unit _attackingUnit;
    private List<Unit> _defendingUnits;
    private List<Tile> _affectedTiles;
    private ScriptableMove _move;
    private Vector2Int _targetedTile;
    public override async void EnterState(GameStateManager gameStateManager)
    {
        _attackingUnit = GameManager.Instance.SelectedUnit;
        _move = GameManager.Instance.SelectedMove;
        _targetedTile = GameManager.Instance.TargetedTile;

        _affectedTiles = GameManager.Instance.GetTargetedTiles(_move, _targetedTile);
        _defendingUnits = GameManager.Instance.GetTargetedUnits(_move, _targetedTile);
        _defendingUnits.RemoveAll(unit => unit.State == Unit.UnitState.Fainted);
        _defendingUnits.Remove(_attackingUnit);

        
        await PlayHurtAnimations(_defendingUnits);

        GameManager.Instance.AlertHurtUnits(_defendingUnits, _move, _targetedTile);

        GameManager.Instance.TransitionState(gameStateManager.checkupState);
    }

    private async Task PlayHurtAnimations(List<Unit> defendingUnits) {
        // Create a list to store all animation tasks
        List<Task> animationTasks = new List<Task>();
        
        // Start all hurt animations concurrently
        foreach (var unit in defendingUnits) {
            animationTasks.Add(unit.PlayAnimationAsync("hurt"));
        }
        
        // Wait for all animations to complete
        await Task.WhenAll(animationTasks);
        AudioController.Instance.PlaySFX(Resources.Load<AudioClip>("Audio/SFX/MiscSFX/Damage"));
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
