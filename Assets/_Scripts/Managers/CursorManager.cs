using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D standardCursor;
    public Texture2D player1Cursor;
    public Texture2D player2Cursor;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.SetCursor(standardCursor, hotSpot, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
