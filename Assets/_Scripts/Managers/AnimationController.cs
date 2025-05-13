using UnityEngine;

public class AnimationController : MonoBehaviour
{
    void Awake()
    {
        //GameManager.OnUnitAttack += OnUnitAttack;
        GameManager.OnUnitHurt += OnUnitHurt;
    }

    public void OnUnitAttack(Unit attacker, ScriptableMove move, Vector2Int mouseTile) {
        attacker.PlayAnimation(move.animationKey, attacker.GetCurrentDirection(), false);
    }

    public void OnUnitHurt(Unit target, ScriptableMove move, Vector2Int mouseTile) {
        //Unit.Direction direction = MovementUtility.GetDirection(target.GetGridPosition(), mouseTile);
        target.PlayAnimation("Hurt", false);
    }
}
