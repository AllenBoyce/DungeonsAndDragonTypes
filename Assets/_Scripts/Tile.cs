//Based on: https://www.youtube.com/watch?v=kkAjpQAM-jE

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using Object = UnityEngine.Object;

// Keeps track of a single Tile on the grid.
public class Tile : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _pathPreview;
    [SerializeField] private LevelManager _levelManager;
    public List<string> tags = new List<string>();
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public bool blocksMovement = false;

    public int x;
    public int y;

    public UnityEvent onTileClicked;
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
        SetHighlight(false);
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }
    private void OnMouseOver()
    {
        _highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    public bool IsOccupied() {
        Debug.Log("Tile IsOccupied: " + x + " " + y + " " + _levelManager.name);
        return _levelManager.GetUnitAt(new Vector2Int(x, y)) != null;
    }

    public void SetHighlight(bool highlight)
    {
        _highlight.SetActive(highlight);
    }

    public void SetPathPreview(bool preview)
    {
        _pathPreview.SetActive(preview);
    }

    public void SetHighlightColor(Color color)
    {
        Color transparentColor = new Color(color.r, color.g, color.b, 90/255f);
        _highlight.GetComponent<SpriteRenderer>().color = transparentColor;
    }
}