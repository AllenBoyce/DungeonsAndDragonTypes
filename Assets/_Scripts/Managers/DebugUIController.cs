using UnityEngine;
using TMPro;
public class DebugUIController : MonoBehaviour
{
    public Canvas _debugCanvas;
    public TextMeshProUGUI _stateText;
    public TextMeshProUGUI _activePlayerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _debugCanvas.gameObject.SetActive(Constants.DebugMode);
        _stateText = _debugCanvas.transform.Find("StateText").GetComponent<TextMeshProUGUI>();
        _activePlayerText = _debugCanvas.transform.Find("PlayerText").GetComponent<TextMeshProUGUI>();
        GameManager.OnGameStateChanged += OnGameStateChanged;
        GameManager.OnActivePlayerChanged += OnActivePlayerChanged;
        string activePlayer = GameManager.Instance.ActivePlayer == 0 ? "Player 1" : "Player 2";
        _activePlayerText.text = $"Active Player: {activePlayer}";
        string stateName = GameManager.Instance.CurrentState.GetType().Name;
        _stateText.text = $"Current State: {stateName}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
    
    void OnGameStateChanged(GameBaseState newState) {
        Debug.Log("DebugUIController: OnGameStateChanged: " + newState.GetType().Name);
        string stateName = newState.GetType().Name;
        _stateText.text = $"Current State: {stateName}";
    }

    void OnActivePlayerChanged(int activePlayer) {
        string player = activePlayer == 0 ? "Player 1" : "Player 2";
        _activePlayerText.text = $"Active Player: {player}";
    }
}
