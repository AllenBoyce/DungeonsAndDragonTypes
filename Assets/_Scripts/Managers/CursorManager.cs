using UnityEngine;

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
        Cursor.SetCursor(standardCursor, hotSpot, cursorMode);
    }

    public void SetCursor(int player) {
        if(player == 0) {
            Cursor.SetCursor(player1Cursor, hotSpot, cursorMode);
        } else {
            Cursor.SetCursor(player2Cursor, hotSpot, cursorMode);
        }
    }
    public void SetStandardCursor() {
        Cursor.SetCursor(standardCursor, hotSpot, cursorMode);
    }

    public void UpdatePlayerCursor() {
        SetCursor(GameManager.Instance.ActivePlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
