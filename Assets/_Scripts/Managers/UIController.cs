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
    [SerializeField] private GameObject _endGameDisplay;
    [SerializeField] private APParent _apParent;
    [SerializeField] private GameObject _selectedUnitIndicator;
    [SerializeField] private TextMeshProUGUI _activePlayerDisplayText;
    public static UIController Instance;

    void Awake()
    {
        Instance = this;
        //_healthBars = new Dictionary<Unit, GameObject>();
        GameManager.OnGameStateChanged += OnGameStateChanged;
        GameManager.OnHoveredTileChanged += OnHoveredTileChanged;
        GameManager.OnUnitSelected += OnUnitSelected;
        GameManager.OnEndGame += OnEndGame;
    }

    void OnDestroy() {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        GameManager.OnHoveredTileChanged -= OnHoveredTileChanged;
        GameManager.OnUnitSelected -= OnUnitSelected;
        GameManager.OnEndGame -= OnEndGame;
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
                Debug.Log("UI Controller receives UnitSelectedState");
                break;
            case "MoveSelectedState":
                OnMoveSelected();
                break;
            case "WalkSelectedState":
                OnWalkSelected();
                break;
            case "ExecuteMoveState":
                OnUnitAttacking();
                break;
        }
    }

    void OnWalkSelected() {
        ClearHighlightedTiles(GameManager.Instance.Grid);
        DisplayAP();
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
                //Debug.Log("UIController OnHoveredTileChanged: " + hoveredTile);
                HighlightTargetedTiles(GameManager.Instance.SelectedMove, hoveredTile, GameManager.Instance.SelectedUnit, GameManager.Instance.Grid);
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
        if (path == null || path.Pivots == null || path.Pivots.Count == 0) return;
        
        // Clear any existing path previews
        ClearHighlightedTiles(GameManager.Instance.Grid);
        
        // Show path preview for each tile in the path
        foreach (Vector2Int pivot in path.Pivots) {
            if (GameManager.Instance.Grid.TryGetValue(pivot, out Tile tile)) {
                tile.SetPathPreview(true);
            }
        }
    }

    public void ClearMovementPath() {
        foreach (Tile tile in GameManager.Instance.Grid.Values) {
            tile.SetPathPreview(false);
        }
    }

    void Start()
    {

    } 

    public void Initialize()
    {
        InitializeEndTurnButton();
        InitializeUnitActionButtons(GameManager.Instance.Units);
        //InitializeHealthBars(GameManager.Instance.Units);
    }

    private void InitializeUnitActionButtons(List<Unit> units) {
        _unitActionButtons = GenerateActionButtons(units);
    }

    private void OnPlayerNeutral() {
        Wipe();
        UpdateActivePlayerDisplay();
    }

    private void UpdateActivePlayerDisplay() {
        _activePlayerDisplayText.text = GameManager.Instance.ActivePlayer == 0 ? "1" : "2";
        Debug.Log(Constants.PLAYER_COLORS[GameManager.Instance.ActivePlayer]);
        _activePlayerDisplayText.color = Constants.PLAYER_COLORS[GameManager.Instance.ActivePlayer];
    }

    private void OnUnitSelected(Unit u) {
        Debug.Log("UI Controller receives OnUnitSelected");

        //Debug.Log(u.name);
        Debug.Log("Displaying Unit Controls");
        DisplayUnitControls(u);
        _apParent.UpdateAP(u);
        DisplayAP();
        _unitActionButtons[u].ForEach(button => button.SetActive(true));
        DisplayUnitIndicator();
        Debug.Log("UIController OnUnitSelected: " + u.name);
    }

    private void OnMoveSelected() {
        Unit u = GameManager.Instance.SelectedUnit;
        ScriptableMove move = GameManager.Instance.SelectedMove;
        SetTempAP(move.apCost);
        DisplayAP();
    }

    public void SetTempAP(int apCost) {
        _apParent.SetTempAP(apCost);
        _apParent.ShowAP();
    }

    public void Wipe()
    {
        WipeUnitControls();
        ClearHighlightedTiles(GameManager.Instance.Grid);
        HideSelectedUnitIndicator();
    }

    private void WipeUnitControls()
    {
        WipeActionButtons();
        WipeAP();
    }

    private void WipeAP()
    {
        _apParent.ClearAP();
    }

    private void DisplayAP(int fullAP, int tempAP)
    {
        _apParent.SetAP(fullAP, tempAP);
        _apParent.ShowAP();
    }

    private void DisplayAP(int fullAP)
    {
        _apParent.SetAP(fullAP);
        _apParent.ShowAP();
    }

    public void DisplayTempAP(int tempAP)
    {
        _apParent.DisplayTempAP(tempAP);
    }

    private void DisplayAP()
    {
        _apParent.ShowAP();
    }
    public void DisplayUnitIndicator()
    {
        DisplaySelectedUnitIndicator(GameManager.Instance.SelectedUnit);
    }
    private void DisplaySelectedUnitIndicator(Unit u) {
        _selectedUnitIndicator.SetActive(true);
        _selectedUnitIndicator.transform.position = new Vector3(u.transform.position.x, u.transform.position.y + 1.35f , Constants.UNIT_INDICATOR_LAYER);
        Debug.Log("UIController DisplaySelectedUnitIndicator: " + u.name);
    }
    private void HideSelectedUnitIndicator() {
        _selectedUnitIndicator.SetActive(false);
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
            //_actionButtons = null;
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
       // DisplayAP(u.GetAP());
        
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
        List<GameObject> actionButtons = new List<GameObject>();
        List<ScriptableMove> learnedMoves = u.GetLearnedMoves();
        if (learnedMoves != null && learnedMoves.Count > 0)
        {
            foreach (var action in learnedMoves)
            {
                Debug.Log(action.name);
                int apCost = action.apCost;
                GameObject actionButton = GenerateActionButton(u, action.name, apCost);
                actionButton.name = $"{u.name}{action.name} Button";
            
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
        Button button = actionButton.GetComponent<Button>();
            
        TMPro.TextMeshProUGUI buttonText = actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = actionName;
            Debug.Log(buttonText.text);
        }
            
        button.onClick.AddListener(() => {
            _gameManager.SelectMove(u, actionName);
        });
        //I would really love to add a mouse over event to this button, but I can't figure out how to do it. If I had it it would preview the AP loss for the move.
        actionButton.SetActive(false);
        return actionButton;
    }

    private GameObject GenerateActionButton(Unit u, string actionName, int apCost)
    {
        GameObject actionButton = Instantiate(_actionButtonPrefab, _gameCanvas.transform.Find("ActionButtons"));
        actionButton.SetActive(false);
        actionButton.name = $"{actionName} Button";
        Button button = actionButton.GetComponent<Button>();
            
        TMPro.TextMeshProUGUI buttonText = actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = $"{actionName} ({apCost})";
            Debug.Log(buttonText.text);
        }
            
        button.onClick.AddListener(() => {
            _gameManager.SelectMove(u, actionName);
        });
        //I would really love to add a mouse over event to this button, but I can't figure out how to do it. If I had it it would preview the AP loss for the move.
        actionButton.SetActive(false);
        return actionButton;
    }

    
    //The spacing portion of this was done by Claude since it involved math for positioning. This was when I had intended on implementing several action buttons, so I was anticipating needing complicated positioning stuff like this.
    private void DisplayActionButtons(Unit u)
    {
        Debug.Log("Displaying Action Buttons");
        Debug.Log("Before: _actionButtons = " + (_actionButtons == null ? "null" : _actionButtons.Count.ToString()));
        _actionButtons = new List<GameObject>();
        List<GameObject> actionButtons = _unitActionButtons[u];
        Debug.Log("actionButtons.Count = " + actionButtons.Count);
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
        float bottomPadding = 20f;
        
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
        _endTurnButton.SetActive(true);
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
        foreach (Tile tile in grid.Values)
        {
            tile.SetHighlight(false);
            tile.SetHighlightColor(Color.white);
        }
    }

    public void HighlightTargetedTiles(ScriptableMove move, Vector2Int targeted, Unit unit, Dictionary<Vector2Int, Tile> grid)
    {
        //sooo rudimentary and please pelase remove later
        ClearHighlightedTiles(grid);
        Debug.Log("UIController HighlightTargetedTiles: " + targeted + " " + unit + " " + move);
        List<Tile> targetedTiles = TargetingUtility.GetTiles(grid, targeted, unit, move);
        foreach (Tile tile in targetedTiles)
        {
            tile.SetHighlightColor(Color.red);
            tile.SetHighlight(true);
        }
    }

    public void OnEndGame(int winner) {
        DisplayEndGame(winner);
    }

    public void DisplayEndGame(int winner) {
        _endTurnButton.SetActive(false);
        _endGameDisplay.SetActive(true);
        _endGameDisplay.transform.Find("EndGameText").GetComponent<TextMeshProUGUI>().text = winner == 0 ? "Player 1 Wins!" : "Player 2 Wins!";
        _endGameDisplay.transform.Find("MenuBtn").gameObject.SetActive(true);
    }
}