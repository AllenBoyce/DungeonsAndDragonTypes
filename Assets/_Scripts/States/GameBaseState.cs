//https://www.youtube.com/watch?v=Vt8aZDPzRjI
using UnityEngine;

public abstract class GameBaseState
{
    public abstract void EnterState(GameStateManager gameStateManager);
    
    public abstract void UpdateState(GameStateManager gameStateManager);

    public abstract void OnCollisionEnter(GameStateManager gameStateManager);
}

public class PlayerNeutralState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void OnCollisionEnter(GameStateManager gameStateManager)
    {
        
    }
}

public class UnitSelectedState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void OnCollisionEnter(GameStateManager gameStateManager)
    {
        
    }
}

public class ChooseTargetState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void OnCollisionEnter(GameStateManager gameStateManager)
    {
        
    }
}

public class ExecuteMoveState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void OnCollisionEnter(GameStateManager gameStateManager)
    {
        
    }
}

public class CheckupState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void OnCollisionEnter(GameStateManager gameStateManager)
    {
        
    }
}