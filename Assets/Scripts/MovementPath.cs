using UnityEngine;

public class MovementPath : MonoBehaviour
{
    //List of all coordinate pairs where the path changes direction. To walk along a path, a unit moves in a straight line from one pivot coordinate to the next
    private List<Vector2Int> _pivots;

    void Start()
    {
        
    }

    public List<Vector2Int> Pivots {
        get {
            return _pivots
        }
        
        set {
            _pivots = value
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
