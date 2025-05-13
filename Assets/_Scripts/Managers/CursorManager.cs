using UnityEngine;
using UnityEngine.SceneManagement;
public class CursorManager : MonoBehaviour
{
    public Texture2D standardCursor;
    public Texture2D player1Cursor;
    public Texture2D player2Cursor;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;
    public static CursorManager Instance;

    void Awake() {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Cursor.SetCursor(standardCursor, hotSpot, cursorMode);
        Debug.Log("CursorManager Awake, setting cursor to standard");
    }

    public void SetCursor(int player) {
        if(player == 0) {
            Cursor.SetCursor(player1Cursor, hotSpot, cursorMode);
            Debug.Log("CursorManager SetCursor, setting cursor to player1");
        } else {
            Cursor.SetCursor(player2Cursor, hotSpot, cursorMode);
            Debug.Log("CursorManager SetCursor, setting cursor to player2");
        }
    }
    public void SetStandardCursor() {
        Cursor.SetCursor(standardCursor, hotSpot, cursorMode);
        Debug.Log("CursorManager SetStandardCursor, setting cursor to standard");
    }

    public void UpdatePlayerCursor() {
        SetCursor(GameManager.Instance.ActivePlayer);
        Debug.Log("CursorManager UpdatePlayerCursor, setting cursor to player " + GameManager.Instance.ActivePlayer);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(scene.name == "CharacterSelect") {
            Debug.Log("CursorManager OnSceneLoaded, setting cursor to player 0");
            SetCursor(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
