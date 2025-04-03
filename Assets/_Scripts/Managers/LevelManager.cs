using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Responsible for keeping track of the Units and the Grid state.
public class LevelManager: MonoBehaviour
{

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private UnitManager _unitManager;
    
    [SerializeField] private List<Unit> _units;
    private Unit _currentUnit;

    void Start()
    {
        _unitManager = FindFirstObjectByType<UnitManager>();
        _gridManager = Object.FindFirstObjectByType<GridManager>();
        _movementController = Object.FindFirstObjectByType<MovementController>();
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }
    
    /**
     * 
     */
    public void PutUnit(Unit u, int x, int y)
    {
        //temporary dumb implementation

        u.transform.position = new Vector3(_gridManager.GetWorldX(x), _gridManager.GetWorldY(y), -5);
        u.SetGridPosition(new Vector2Int(x, y));
        Debug.Log(u.transform.position);
    }

    public Unit GetUnitAt(Vector2Int position)
    {
        foreach (Unit u in _units)
        {
            Vector2Int upos = u.GetGridPosition();
            Debug.Log(upos);
            if (upos.x == position.x && upos.y == position.y) return u;
        }

        return null;
    }

    private static bool IsValidPath(MovementPath path) {
        return path.Pivots.Count > 0;
    }

    public List<Unit> Units { get { return _units; } }

    void Update()
    {
        
    }
}
