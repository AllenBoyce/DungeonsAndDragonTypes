using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private List<ScriptablePokemon> _scriptablePokemon;
    //private Dictionary<PokemonSpecies, string> _nameToPokemon = new Dictionary<PokemonSpecies, string>();
    private Transform _units;
    void Start()
    {
        _levelManager = FindFirstObjectByType<LevelManager>();
        _units = transform.GetChild(0);
    }

    #region Lifecycle
    /**
     * Spawns the Unit at the given position on the grid.
     */
    void SpawnUnit(Unit unit, Vector2Int position)
    {
        _levelManager.PutUnit(unit, position.x, position.y);
        unit.enabled = true;
    }
    
    
    /**
     * Generates a new Unit Object from a ScriptablePokemon and its owner player
     * Returns the newly created Unit
     */
    public Unit GenerateUnit(ScriptablePokemon pokemon, int player)
    {
        //Debug.Log("UnitManager GenerateUnit: " + pokemon.GetName() + " for player " + player);
        // Create a new GameObject for the unit
        GameObject unitObject = new GameObject($"Unit_{pokemon.GetName()}_{player}");
    
        // Add the Unit component to the new GameObject
        Unit unit = unitObject.AddComponent<Unit>();
    
        // Initialize the unit
        Vector3 offScreen = new Vector3(-1, -1, -500);
        Debug.Log("UnitManager GenerateUnit: " + pokemon.GetName() + " for player " + player);
        unit.Initialize(pokemon, offScreen, player);
    
        // Set parent to _units if needed
        if (_units != null)
        {
            unitObject.transform.parent = _units;
        }
    
        // Add to level manager
        _levelManager.AddUnit(unit);
    
        return unit;
    }
    
    /**
     * Generates a new Unit Object from a PokemonSpecies enum and its owner player
     * Returns the newly created Unit
     */
    public Unit GenerateUnit(Constants.PokemonSpecies species, int player)
    {
        ScriptablePokemon pokemon = FindScriptablePokemon(species);
        return GenerateUnit(pokemon, player);
    }

    private ScriptablePokemon FindScriptablePokemon(Constants.PokemonSpecies species)
    {
        foreach (ScriptablePokemon pokemon in _scriptablePokemon)
        {
            if (pokemon.species == species) return pokemon;
        }
        Debug.LogError($"No scriptable pokemon found for species: {species}");
        throw new System.Exception("No scriptable pokemon found for species: " + species);
    }
    
    /**
     * Entirely removes the unit from the grid and from the game.
     */
    public void DeleteUnit(Unit unit)
    {
        
    }
    
    /**
     * Removes the given unit from the grid
     */
    public void DespawnUnit(Unit unit)
    {
        unit.enabled = false;
    }

    #endregion

    
    
}
