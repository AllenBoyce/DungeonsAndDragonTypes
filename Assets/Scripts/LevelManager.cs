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

    public void HandleTileClick(Tile t)
    {
        Debug.Log(("Tile clicked @ {0}, {1}", t.x, t.y));
        if(_isPlayersTurn)
        {
            if (isValidMove(_player, t.x, t.y))
            {
                _movementController.MoveUnit(_player, t.x, t.y);
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
        return true;
    }


    void Update()
    {
        
    }
}
