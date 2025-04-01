using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    GameBaseState currentState;
    PlayerNeutralState playerNeutralState = new PlayerNeutralState();
    UnitSelectedState unitSelectedState = new UnitSelectedState();
    ChooseTargetState chooseTargetState = new ChooseTargetState();
    ExecuteMoveState executeMoveState = new ExecuteMoveState();
    CheckupState checkupState = new CheckupState();

    void Start()
    {
        currentState = playerNeutralState;
        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }
}
