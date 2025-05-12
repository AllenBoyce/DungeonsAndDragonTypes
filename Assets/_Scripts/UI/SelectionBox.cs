using UnityEngine;
using UnityEngine.UI;
using System;
public class SelectionBox : MonoBehaviour
{
    private Button _button;

    [SerializeField] private Constants.PokemonSpecies _pokemonSpecies;
    private GameObject _playerOneSelectionBox;
    private GameObject _playerTwoSelectionBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private SelectionManager _selectionManager;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);

        _playerOneSelectionBox = GameObject.Find("Player1SelectionBox");
        _playerTwoSelectionBox = GameObject.Find("Player2SelectionBox");



        // _playerOneSelectionBox.SetActive(false);
        // _playerTwoSelectionBox.SetActive(false);
        _selectionManager = FindFirstObjectByType<SelectionManager>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick()
    {
        _selectionManager.OnPokemonSelected(_pokemonSpecies);
    }
}
