using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This class is so messy and ugly and stupid and is a horrible example of a singleton but I did it last minute and it works.
public class SelectionManager : PersistentSingleton<SelectionManager>
{
    private int _teamSize = Constants.TEAM_SIZE;
    private int _playerCount = Constants.PLAYER_COUNT;
    private Color[] _playerColors = Constants.PLAYER_COLORS.ToArray();
    private int _currentSelectedUnits = 0;
    private List<Constants.PokemonSpecies> _playerOnePokemon = new List<Constants.PokemonSpecies>();
    private List<Constants.PokemonSpecies> _playerTwoPokemon = new List<Constants.PokemonSpecies>();
    private int _currentPlayer = 0; //0-indexed.

    [SerializeField] public Button _startGameBtn;
    [SerializeField] public Button _undoBtn;

    private TextMeshProUGUI _instructionsText;
    private TextMeshProUGUI _unitCountText;


    public List<Constants.PokemonSpecies> GetPlayerOnePokemon() {
        return _playerOnePokemon;
    }

    public List<Constants.PokemonSpecies> GetPlayerTwoPokemon() {
        return _playerTwoPokemon;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "CharacterSelect") {
            Debug.Log("Scene loaded: CharacterSelect");
             _startGameBtn = GameObject.Find("StartGameBtn").GetComponent<Button>();
            _undoBtn = GameObject.Find("UndoBtn").GetComponent<Button>();
            _startGameBtn = GameObject.Find("StartGameBtn").GetComponent<Button>();
            _undoBtn = GameObject.Find("UndoBtn").GetComponent<Button>();
            _unitCountText = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
            _instructionsText = GameObject.Find("InstructionsText").GetComponent<TextMeshProUGUI>();
            _instructionsText.text = "Select Player 1's Pokemon";
            _unitCountText.text = "(0/" + _teamSize + ")";
            _startGameBtn.interactable = false;
            _undoBtn.interactable = false;
            _undoBtn.onClick.AddListener(OnUndo);
            CursorManager.Instance.SetCursor(0);
            _playerOnePokemon.Clear();
            _playerTwoPokemon.Clear();
            _currentSelectedUnits = 0;
            _currentPlayer = 0;
        }
    }

    void Start()
    {
        _startGameBtn = GameObject.Find("StartGameBtn").GetComponent<Button>();
        _undoBtn = GameObject.Find("UndoBtn").GetComponent<Button>();
        _unitCountText = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
        _unitCountText.text = "(0/" + _teamSize + ")";
        _startGameBtn.onClick.AddListener(OnStartGame);
        _undoBtn.onClick.AddListener(OnUndo);
        _startGameBtn.interactable = false;
        _undoBtn.interactable = false;
        _instructionsText = GameObject.Find("InstructionsText").GetComponent<TextMeshProUGUI>();
        _instructionsText.text = "Select Player 1's Pokemon";
        _currentPlayer = 0;
        CursorManager.Instance.SetCursor(0);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("Current Player: " + _currentPlayer);
                Debug.Log("Player 1 Pokemon: " + _playerOnePokemon.Count);
                Debug.Log("Player 2 Pokemon: " + _playerTwoPokemon.Count);
            }
    }

    void OnStartGame() {
        
    }

    public void Reset() {
        _playerOnePokemon.Clear();
        _playerTwoPokemon.Clear();
        _currentSelectedUnits = 0;
        _currentPlayer = 0;
        _instructionsText.text = "Select Player 1's Pokemon";
        CursorManager.Instance.SetCursor(0);
    }
    void OnUndo() {
        Debug.Log("Undoing");
        _startGameBtn.interactable = false;
        switch (_currentPlayer) {
            case 0:
                if(_playerOnePokemon.Count > 0) {
                    _playerOnePokemon.RemoveAt(_playerOnePokemon.Count - 1);
                    _currentSelectedUnits--;
                    _unitCountText.text = "(" + _currentSelectedUnits + "/" + _teamSize + ")";
                }
                if(_playerOnePokemon.Count == 0) {
                    _undoBtn.interactable = false;
                }
                break;
            case 1:
                if(_playerTwoPokemon.Count > 0) {
                    _playerTwoPokemon.RemoveAt(_playerTwoPokemon.Count - 1);
                    _currentSelectedUnits--;
                    _unitCountText.text = "(" + _currentSelectedUnits + "/" + _teamSize + ")";
                }
                else {
                    _playerOnePokemon.RemoveAt(_playerOnePokemon.Count - 1);
                    SwitchPlayer(0);
                }
                break;
        }
    }


    //There's gotta be a better way to do this but fuck it
   public void OnPokemonSelected(Constants.PokemonSpecies pokemonSpecies) 
    {
        Debug.Log("Pokemon selected: " + pokemonSpecies);
        _undoBtn.interactable = true;

        switch (_currentPlayer)
        {
            case 0: //We're on player 1
            if(_playerOnePokemon.Count >= _teamSize) {
                Debug.Log("Player 1 team is full");
                return;
            }
                _playerOnePokemon.Add(pokemonSpecies);
                _currentSelectedUnits++;
                _unitCountText.text = "(" + _currentSelectedUnits + "/" + _teamSize + ")";
                if(_currentSelectedUnits >= _teamSize)
                {
                    SwitchPlayer(1);
                }
                break;
            case 1: //We're on player 2
                if(_playerTwoPokemon.Count >= _teamSize) {
                    Debug.Log("Player 2 team is full");
                    return;
                }
                _playerTwoPokemon.Add(pokemonSpecies);
                _currentSelectedUnits++;
                _unitCountText.text = "(" + _currentSelectedUnits + "/" + _teamSize + ")";
                if(_currentSelectedUnits >= _teamSize)
                {
                    //Logic for enabling Begin Game button
                    _startGameBtn.interactable = true;
                }
                break;
        }
    }

    public int CurrentPlayer {
        get {
            return _currentPlayer;
        }
    }

    void SwitchPlayer(int nextPlayer) {
        Debug.Log("Switching to player: " + nextPlayer);
        if(nextPlayer == 0) {
            CursorManager.Instance.SetCursor(0);
        } else {
            CursorManager.Instance.SetCursor(1);
        }
        _currentPlayer = nextPlayer;
        switch (nextPlayer) {
            case 0:
                _instructionsText.text = "Select Player 1's Pokemon";
                _currentSelectedUnits = _playerOnePokemon.Count;
                break;
            case 1:
                _instructionsText.text = "Select Player 2's Pokemon";
                _currentSelectedUnits = _playerTwoPokemon.Count;
                break;
        }
        _unitCountText.text = "(" + _currentSelectedUnits + "/" + _teamSize + ")";
        Debug.Log(_unitCountText.text);

    }

    

}
