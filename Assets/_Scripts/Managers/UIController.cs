using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class UIController : MonoBehaviour
{
    [SerializeField] private Canvas _gameCanvas;
    private GameObject _portraitObj;
    private List<GameObject> _actionButtons;
    private Dictionary<Unit, List<GameObject>> _unitActionButtons; //Each unit gets a list of action buttons, generated at the beginning of the game.
    
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _actionButtonPrefab;
    [SerializeField] private GameObject _endTurnButton;

    void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
        GameManager.OnHoveredTileChanged += OnHoveredTileChanged;
        GameManager.OnUnitSelected += OnUnitSelected;
    }

    void OnDestroy() {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        GameManager.OnHoveredTileChanged -= OnHoveredTileChanged;
        GameManager.OnUnitSelected -= OnUnitSelected;
    }

    #region Event Listeners
    void OnGameStateChanged(GameBaseState newState) {
        Wipe();
        switch (newState.GetType().Name) { //Ugly and lame but works
            case "PlayerNeutralState":
                Debug.Log("UI Controller receives PlayerNeutralState");
                OnPlayerNeutral();
                break;
            case "UnitSelectedState":
                OnUnitSelected(GameManager.Instance.SelectedUnit);
                break;
            case "MoveSelectedState":
                OnMoveSelected();
                break;
            case "ExecuteMoveState":
                OnUnitAttacking();
                break;
        }
    }

    void OnUnitAttacking() {
        ClearHighlightedTiles(GameManager.Instance.Grid);
    }


    void OnHoveredTileChanged(Vector2Int hoveredTile) {
        //Debug.Log(hoveredTile);
        switch (GameManager.Instance.CurrentState.GetType().Name) { //Ugly and lame but works
            case "PlayerNeutralState":
                break;
            case "UnitSelectedState":
                break;
            case "MoveSelectedState":
                HighlightTargetedTiles(GameManager.Instance.SelectedMove, hoveredTile, GameManager.Instance.SelectedUnit.GetCurrentDirection(), GameManager.Instance.Grid);
                break;
            case "ExecuteMoveState":
                break;
        }
    }

    #endregion

    /**
     * Displays a preview of the movement path for the selected unit.
     * The path is displayed as a series of PathPreview sprites above each tile in the path.
     */
    public void PreviewMovementPath(MovementPath path) {
        
    }

    // void Start()
    // {
    //     Initialize();
    // }

    public void Initialize()
    {
        InitializeEndTurnButton();
        InitializeUnitActionButtons(GameManager.Instance.Units);
    }

    private void InitializeUnitActionButtons(List<Unit> units) {
        _unitActionButtons = GenerateActionButtons(units);
        foreach (Unit u in units) {
            Debug.Log(u.name);
        }

    }

    private void OnPlayerNeutral() {
        Wipe();
    }

    private void OnUnitSelected(Unit u) {
        
        Debug.Log(u.name);
        DisplayUnitControls(u);

        _unitActionButtons[u].ForEach(button => button.SetActive(true));
    }

    private void OnMoveSelected() {
        Unit u = GameManager.Instance.SelectedUnit;
        ScriptableMove move = GameManager.Instance.SelectedMove;
    }

    public void Wipe()
    {
        WipeUnitControls();
        ClearHighlightedTiles(GameManager.Instance.Grid);
    }

    private void WipeUnitControls()
    {
        WipeActionButtons();
    }

    private void WipeActionButtons()
    {
        // Destroy all action buttons
        if (_actionButtons != null)
        {
            foreach(GameObject button in _actionButtons)
            {
                if (button != null)
                {
                    button.SetActive(false);
                }
            }
            _actionButtons = null;
        }
    }

    /**
     * Puts the Unit portrait at the top of the screen
     * Puts the Unit's move buttons at the bottom of the screen
     */
    public void DisplayUnitControls(Unit u)
    {
        ScriptablePokemon data = u.GetPokemonData();
        DisplayUnitPortrait(data.portrait);
        List<ScriptableMove> learnedMoves = new List<ScriptableMove>();
        DisplayActionButtons(u);
        
        _endTurnButton.SetActive(true);
    }
    
    private Dictionary<Unit, List<GameObject>> GenerateActionButtons(List<Unit> units) {
        Dictionary<Unit, List<GameObject>> unitActionButtons = new Dictionary<Unit, List<GameObject>>();
        foreach (Unit u in units) {
            unitActionButtons[u] = GenerateUnitActionButtons(u);
        }
        return unitActionButtons;
    }

    /**
     * Generated a list of Button GameObjects that instantiate the ActionButton prefab.
     * Each ActionButton corresponds to one of the given Unit's learnedMoves.
     * There is one additional ActionButton at the beginning of the list that corresponds to the "Move" Action.
     * The text of each Button is the name of the Action it refers to.
     * When the Button is clicked, it calls GameManager's SelectMove function, passing in the unit and the ScriptableMove.
     */
    private List<GameObject> GenerateUnitActionButtons(Unit u)
    {
        // Create a new list to store the generated button GameObjects
        List<GameObject> actionButtons = new List<GameObject>();
        List<ScriptableMove> learnedMoves = u.GetLearnedMoves();
        // Check if the unit has any actions
        if (learnedMoves != null && learnedMoves.Count > 0)
        {
            // Iterate through each action the unit has
            foreach (var action in learnedMoves)
            {
                Debug.Log(action.name);
                // Instantiate a button prefab from the UI resources
                
                GameObject actionButton = GenerateActionButton(u, action.name);
                actionButton.name = $"{u.name}{action.name} Button";
            
                // Add the button to our list
                actionButtons.Add(actionButton);
            }
        }                
        actionButtons.Insert(0, GenerateActionButton(u, "Move"));
        _actionButtons = new List<GameObject>(actionButtons);
        return actionButtons;
    }

    private GameObject GenerateActionButton(Unit u, string actionName)
    {
        GameObject actionButton = Instantiate(_actionButtonPrefab, _gameCanvas.transform.Find("ActionButtons"));
        actionButton.SetActive(false);
        actionButton.name = $"{actionName} Button";
        // Get the button component
        Button button = actionButton.GetComponent<Button>();
            
        // Set button text to action name
        TMPro.TextMeshProUGUI buttonText = actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = actionName;
            Debug.Log("71");
            Debug.Log(buttonText.text);
        }
            
        // Add onClick listener that will execute the action when clicked
        button.onClick.AddListener(() => {
            Debug.Log($"CLICK {actionName} BTN");
            Debug.Log(_gameManager.name);
            _gameManager.SelectMove(u, actionName);
        });
        actionButton.SetActive(false);
        return actionButton;
    }

    
    private void DisplayActionButtons(Unit u)
    {
        _actionButtons = new List<GameObject>();
        List<GameObject> actionButtons = _unitActionButtons[u];
        Debug.Log(actionButtons.Count);
        // If no buttons to display, return early
        if (actionButtons == null || actionButtons.Count == 0)
            return;
    
        // Get the canvas rect to help with positioning
        RectTransform canvasRect = _gameCanvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
    
        // Calculate total width needed for all buttons
        float totalButtonWidth = 0;
        float maxButtonHeight = 0;
    
        // Get button dimensions and calculate total width needed
        List<float> buttonWidths = new List<float>();
        foreach (GameObject button in actionButtons)
        {
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            float buttonWidth = buttonRect.rect.width;
            float buttonHeight = buttonRect.rect.height;
        
            buttonWidths.Add(buttonWidth);
            totalButtonWidth += buttonWidth;
            maxButtonHeight = Mathf.Max(maxButtonHeight, buttonHeight);
        }
    
        // Calculate spacing between buttons (assuming we want equal spacing)
        float buttonSpacing = 20f;
        float totalWidth = totalButtonWidth + buttonSpacing * (actionButtons.Count - 1);
    
        // Calculate starting X position to center all buttons
        float startX = -totalWidth / 2;
        float currentX = startX;
    
        // Set position near bottom of screen with padding
        float bottomPadding = 50f;
        
        // Position each button
        for (int i = 0; i < actionButtons.Count; i++)
        {
            GameObject button = actionButtons[i];
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            
            // Set anchors to bottom of screen
            buttonRect.anchorMin = new Vector2(0.5f, 0f);
            buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
        
            // Position the button with positive Y value from bottom
            buttonRect.anchoredPosition = new Vector2(currentX + buttonWidths[i] / 2, bottomPadding);
        
            // Set the button as active
            button.SetActive(true);
        
            // Move to the next button position
            currentX += buttonWidths[i] + buttonSpacing;
            _actionButtons.Add(button);
        }
    }

    private void InitializeEndTurnButton()
    {
        _endTurnButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("End Turn Button Clicked");
            _gameManager.EndTurn();
        });
        
    }

    private void DisplayUnitPortrait(Sprite portrait)
    {
        if (portrait == null)
        {
            Debug.LogWarning("Portrait sprite is null");
            return;
        }

        if (_portraitObj == null)
        {
            Debug.LogWarning("Portrait object is null");
            return;
        }

        _portraitObj.SetActive(true);
        
        // Use the fully qualified name to avoid any confusion
        UnityEngine.UI.Image portraitImage = _portraitObj.GetComponent<UnityEngine.UI.Image>();
        
        if (portraitImage == null)
        {
            Debug.LogError("No Image component found on portrait object");
            return;
        }
        
        portraitImage.sprite = portrait;
        Debug.Log("Portrait updated to: " + portrait.name);
    }


    public void ClearHighlightedTiles(Dictionary<Vector2Int, Tile> grid)
    {
        foreach (Tile tile in grid.Values) tile.SetHighLight(false);
    }

    public void HighlightTargetedTiles(ScriptableMove move, Vector2Int origin, Unit.Direction direction, Dictionary<Vector2Int, Tile> grid)
    {
        //sooo rudimentary and please pelase remove later
        ClearHighlightedTiles(grid);
        List<Tile> targetedTiles = TargetingUtility.GetTiles(grid, origin, direction, move);
        foreach (Tile tile in targetedTiles)
        {
            tile.SetHighLight(true);
        }
    }
}
