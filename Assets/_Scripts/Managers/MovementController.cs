using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private GridManager _gridManager;
    private float _moveSpeed = 2.0f;

    public static event Action<Unit, MovementPath> OnUnitMoving;
    public static event Action<Unit, MovementPath> OnUnitStoppedMoving;

    void Start()
    {
        _gridManager = FindFirstObjectByType<GridManager>();
    }

    public async Task WalkUnit(Unit u, MovementPath path) {
        OnUnitMoving?.Invoke(u, path);
        u.UpdateState(Unit.UnitState.Moving);
        if(path.Pivots.Count <= 0){
            Debug.LogError("MovementPath has no pivots");
            return;
        }
        float unitZ = u.transform.position.z;

        
        
        foreach(Vector2Int point in path.Pivots) {
            Vector2Int unitTile = u.GetGridPosition();
            Vector3 destination = getDestination(point.x, point.y, unitZ);
            Unit.Direction direction = MovementUtility.GetDirection(unitTile, point);
            u.PlayAnimation("Walk", direction);
            await DragUnit(u, destination);
            u.SetGridPosition(point);
        }
        u.PlayAnimation("Idle");
        u.UpdateState(Unit.UnitState.Idle);
        OnUnitStoppedMoving?.Invoke(u, path);
    }

    

    // public async void MoveUnit(Unit u, int gridX, int gridY)
    // {
    //     u.State = "MOVING";
    //     await MoveUnitVertical(u, gridY);
    //     await MoveUnitHorizontal(u, gridX);
    //     u.State = "IDLE";
    // }

    public async Task MoveUnitHorizontal(Unit u, int gridX)
    {
        Vector3 destination = getHorizontalDestination(u, gridX);
        await DragUnit(u, destination);
    }

    public async Task MoveUnitVertical(Unit u, int gridY)
    {
        Vector3 destination = getVerticalDestination(u, gridY);
        await DragUnit(u, destination);
    }

    /**
     * Smoothly moves the unit to the *world* location at the given destination
     */
    public async Task DragUnit(Unit u, Vector3 destination)
    {
        float speed = 2.0f;
        float distance = Vector3.Distance(u.transform.position, destination);
        float duration = distance / speed;
        float elapsedTime = 0f;

        Vector3 startPosition = u.transform.position;

        while (elapsedTime < duration)
        {
            u.transform.position = Vector3.Lerp(startPosition, destination, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        u.transform.position = destination;
    }

    private Vector3 getHorizontalDestination(Unit u, int gridX)
    {
        float destX = _gridManager.GetWorldX(gridX);
        return new Vector3(destX, u.transform.position.y, u.transform.position.z);
    }
    private Vector3 getVerticalDestination(Unit u, int gridY)
    {
        float destY = _gridManager.GetWorldY(gridY);
        return new Vector3(u.transform.position.x, destY, u.transform.position.z);
    }

    private Vector3 getDestination(int gridX, int gridY, float unitZ) {
        float destX = _gridManager.GetWorldX(gridX);
        float destY = _gridManager.GetWorldY(gridY);
        return new Vector3(destX, destY, unitZ);
    }
}
