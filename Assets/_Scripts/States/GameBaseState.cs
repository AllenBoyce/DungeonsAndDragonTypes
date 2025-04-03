//https://www.youtube.com/watch?v=Vt8aZDPzRjI
using UnityEngine;

public abstract class GameBaseState
{
    public abstract void EnterState(GameStateManager gameStateManager);
    
    public abstract void UpdateState(GameStateManager gameStateManager);

    public abstract void OnCollisionEnter(GameStateManager gameStateManager);
}
