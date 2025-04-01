using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // Add this import for Image component
using Button = UnityEngine.UI.Button;

public class UIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Canvas _gameCanvas;
    private GameObject _portraitObj;
    private Button[] _actionButtons;
    
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _actionButtonPrefab;
    void Start()
    {
        _gameCanvas = GameObject.Find("GameCanvas").GetComponent<Canvas>();
    }

    public void Wipe()
    {
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
                GameObject actionButton = Instantiate(_actionButtonPrefab, _gameCanvas.transform.Find("ActionButtons"));
                actionButton.SetActive(false);
                actionButton.name = $"{action.name} Button";
                // Get the button component
                Button button = actionButton.GetComponent<Button>();
            
                // Set button text to action name
                TMPro.TextMeshProUGUI buttonText = actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = action.name;
                    Debug.Log("71");
                    Debug.Log(buttonText.text);
                }
            
                // Add onClick listener that will execute the action when clicked
                button.onClick.AddListener(() => {
                    Debug.Log($"CLICK {action.name} BTN");
                    Debug.Log(_gameManager.name);
                    _gameManager.SelectMove(u, action.name);
                });
            
                // Add the button to our list
                actionButtons.Add(actionButton);
            }
        }
    
        return actionButtons;
    }

    
    private void DisplayActionButtons(Unit u)
    {
        foreach (GameObject button in GenerateActionButtons(u))
        {
            //Place button where it needs to be
            button.SetActive(true);
        }
    }
    
    /**
     * Generates a list of Buttons to place on the canvas.
     * Each Button will correspond to a Unit's Moves, and will call the GameManager's SelectMove function.
     */
    private List<GameObject> GetMoveButtons(Unit u)
    {
        List<GameObject> moveButtons = new List<GameObject>();
        ScriptablePokemon pokemonData = u.GetPokemonData();
        
        if (pokemonData == null || pokemonData.learnableMoves == null)
        {
            Debug.LogWarning("Pokemon data or learnable moves is null");
            return moveButtons;
        }
        
        // Find the move buttons parent container
        Transform buttonContainer = _gameCanvas.transform.Find("MoveButtonsContainer");
        if (buttonContainer == null)
        {
            Debug.LogWarning("MoveButtonsContainer not found in canvas");
            return moveButtons;
        }
        
        // Get the button prefab
        GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/UI/MoveButton");
        if (buttonPrefab == null)
        {
            Debug.LogError("Move button prefab not found");
            return moveButtons;
        }
        
        // Create a button for each move
        foreach (ScriptableMove move in pokemonData.learnableMoves)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            
            // Set button text
            TMPro.TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = move.moveName;
                Debug.Log(buttonText.text);
            }
            
            // Set button color based on move type
            // This could be expanded with a color map for each move type
            
            // Add click event
            button.onClick.AddListener(() => {
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.SelectMove(u, move.moveName);
                }
            });
            
            moveButtons.Add(buttonObj);
        }
        
        return moveButtons;
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
}
