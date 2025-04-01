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
        Unit unit = this.AddComponent<Unit>();
        Vector3 offScreen = new Vector3(-1, -1, -500);
        unit.Initialize(pokemon, offScreen, player);
        unit.transform.parent = _units;
        _levelManager.AddUnit(unit);
        return unit;
    }
    
    /**
     * Generates a new Unit Object from a PokemonSpecies enum and its owner player
     * Returns the newly created Unit
     */
    public Unit GenerateUnit(string species, int player)
    {
        ScriptablePokemon pokemon = FindScriptablePokemon(species);
        
        return GenerateUnit(pokemon, player);
    }

    private ScriptablePokemon FindScriptablePokemon(string species)
    {
        foreach (ScriptablePokemon pokemon in _scriptablePokemon)
        {
            if (pokemon.name == species) return pokemon;
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
