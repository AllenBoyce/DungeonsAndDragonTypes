using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class UIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Canvas _gameCanvas;
    private GameObject _portraitObj;
    private List<GameObject> _actionButtons;
    
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _actionButtonPrefab;
    [SerializeField] private GameObject _endTurnButton;
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        InitializeEndTurnButton();
    }

    public void Wipe()
    {
        WipeUnitControls();
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
                    Destroy(button);
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
    
    /**
     * Generated a list of Button GameObjects that instantiate the ActionButton prefab.
     * Each ActionButton corresponds to one of the given Unit's learnedMoves.
     * There is one additional ActionButton at the beginning of the list that corresponds to the "Move" Action.
     * The text of each Button is the name of the Action it refers to.
     * When the Button is clicked, it calls GameManager's SelectMove function, passing in the unit and the ScriptableMove.
     */
    private List<GameObject> GenerateActionButtons(Unit u)
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

        return actionButton;
    }

    
    private void DisplayActionButtons(Unit u)
    {
        List<GameObject> actionButtons = GenerateActionButtons(u);
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
        // Let's use a fixed amount of spacing between buttons 
        float buttonSpacing = 20f;
        float totalWidth = totalButtonWidth + buttonSpacing * (actionButtons.Count - 1);
    
        // Calculate starting X position to center all buttons
        float startX = -totalWidth / 2;
        float currentX = startX;
    
        // Set Y position as requested
        float yPosition = -312f;
    
        // Position each button
        for (int i = 0; i < actionButtons.Count; i++)
        {
            GameObject button = actionButtons[i];
            RectTransform buttonRect = button.GetComponent<RectTransform>();
        
            // Position the button
            buttonRect.anchoredPosition = new Vector2(currentX + buttonWidths[i] / 2, yPosition);
        
            // Set the button as active
            button.SetActive(true);
        
            // Move to the next button position
            currentX += buttonWidths[i] + buttonSpacing;
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
