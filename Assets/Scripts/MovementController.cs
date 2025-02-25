using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private GridManager _gridManager;
    void Start()
    {
        _gridManager = Object.FindFirstObjectByType<GridManager>();
    }

    public async Task WalkUnit(Unit u, MovementPath path) {
        u.IsMoving = true;
        if(path.Pivots.Count <= 0){
            Debug.LogError("MovementPath has no pivots");
            return;
        }
        float unitZ = u.transform.position.z;
        foreach(Vector2Int point in path.Pivots) {
            Vector3 destination = getDestination(point.x, point.y, unitZ);
            await DragUnit(u, destination);
        }
        u.IsMoving = false;
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

    public async Task DragUnit(Unit u, Vector3 destination)
    {
        float speed = u.MoveSpeed;
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
