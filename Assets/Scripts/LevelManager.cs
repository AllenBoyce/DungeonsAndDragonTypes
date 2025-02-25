using System.Threading.Tasks;
using UnityEngine;

public class LevelManager: MonoBehaviour
{

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private Player _player;

    private bool _isPlayersTurn;

    void Start()
    {
        _gridManager = Object.FindFirstObjectByType<GridManager>();
        //_player = Object.FindFirstObjectByType<Player>();
        _isPlayersTurn = true;
        
        putUnit(_player, 4, 4);
    }

    private void putUnit(Unit u, int x, int y)
    {
        //temporary dumb implementation

        u.transform.position = new Vector3(_gridManager.GetWorldX(x), _gridManager.GetWorldY(y), -5);
        u.enabled = true;
    }

    public async Task HandleTileClick(Tile t)
    {
        Debug.Log(("Tile clicked @ {0}, {1}", t.x, t.y));
        Debug.Log(t.blocksMovement);
        if(_isPlayersTurn && !_player.IsMoving)
        {
            Vector2Int origin = _gridManager.GetGridPosition(new Vector2(_player.transform.position.x, _player.transform.position.y));
            Vector2Int destination = new Vector2Int(t.x, t.y);
            MovementPath path = _gridManager.GenerateMovementPath(origin, destination);
            if (isValidPath(path) && _isPlayersTurn) //rewrite logic when turns are implemented
            {
                await _movementController.WalkUnit(_player, path);
                //_isPlayersTurn = false; Removing this for now for testing
            }
            else {
                Debug.Log("Invalid move");
            }
        }
    }

    //Complete later with checks once paths are implemented
    private bool isValidMove(Unit u, int x, int y)
    {
        return !_gridManager.Grid[new Vector2Int(x,y)].blocksMovement;
    }

    private bool isValidPath(MovementPath path) {
        return path.Pivots.Count > 0;
    }

    void Update()
    {
        
    }
}
