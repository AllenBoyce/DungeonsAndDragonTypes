//Based on: https://www.youtube.com/watch?v=kkAjpQAM-jE

using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine.PlayerLoop;

public class Tile : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private LevelManager _levelManager;
    public List<string> tags = new List<string>();
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public bool blocksMovement = false;

    public int x;
    public int y;

    private void Start()
    {
        _gridManager = Object.FindFirstObjectByType<GridManager>();
        if(_gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene.");
        }
        _levelManager = Object.FindFirstObjectByType<LevelManager>();
        if (_levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene.");
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found in the scene.");
        }
    }

    public void SetSprite(Sprite sprite)
    {
        Debug.Log(sprite.name);
        _spriteRenderer.sprite = sprite;
    }
    private void OnMouseOver()
    {
        _highlight.SetActive(true);

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    private void HandleMouseClick()
    {
        _levelManager.HandleTileClick(this);
    }
}