using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Unit : MonoBehaviour
{
    private Transform _transform;
    
    private int _currentHP;
    private int _currentAP;
    private Vector2Int _gridPosition;
    private int playerOwner;
    
    [SerializeField] private ScriptablePokemon pokemonData;
    private List<ScriptableMove> _learnedMoves;
    private PokemonModel _model;

    private Direction _currentDirection;
    public void Start()
    {
        
    }

    public void Initialize(ScriptablePokemon pokemon, Vector3 position, int player)
    {
        pokemonData = pokemon;
        _transform = transform;
        //Hiding model behind camera
        //Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, -500);
        _transform.position = position;
        _model = Instantiate(pokemon.prefab, position, Quaternion.identity, transform);
        _model.name = "Model";
        
        //Temporary:
        _learnedMoves = pokemonData.learnableMoves;
    }
    
    public List<ScriptableMove> GetLearnedMoves() { return _learnedMoves;}
    public Direction GetCurrentDirection() { return _currentDirection; }
    public void SetCurrentDirection(Direction direction) { _currentDirection = direction; }

    public ScriptablePokemon GetPokemonData()
    {
        return pokemonData;
    }

    public List<string> GetLearnedMoveNames()
    {
        List<string> moveNames = new List<string>();
        foreach (ScriptableMove move in _learnedMoves)
        {
            moveNames.Add(move.name);
        }
        return moveNames;
    }

    public Dictionary<string, ScriptableMove> GetMoveDictionary()
    {
        Dictionary<string, ScriptableMove> moveDict = new Dictionary<string, ScriptableMove>();
        foreach (ScriptableMove move in _learnedMoves)
        {
            moveDict.Add(move.name, move);
        }
        return moveDict;
    }
    
    public Vector2Int GetGridPosition()
    {
        return _gridPosition;
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        _gridPosition = gridPosition;
    }
    public int PlayerOwner { get { return playerOwner; } }

    //Temp: loop has no effect
    public void PlayAnimation(string animationName, Direction direction, bool loop = true)
    {
        _currentDirection = direction;
        _model.PlayAnimation(animationName, direction);
    }
    public void PlayAnimation(string animationName, bool loop = true)
    {
        _model.PlayAnimation(animationName, _currentDirection);
    }

    public void Hurt(int damage)
    {
        _state = UnitState.Hurting;
        PlayAnimation("Hurt");
        takeDamage(damage);
    }

    private void takeDamage(int damage)
    {
        _currentHP = Math.Max(0, _currentHP - damage);
        if (_currentHP <= 0)
        {
            Faint();
        }
    }

    public void Faint()
    {
        //PlayAnimation("Faint");
        _state = UnitState.Fainted;
    }

    public enum UnitState
    {
        Idle,
        Moving,
        Attacking,
        Hurting,
        Fainted,
    }

    public enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
    
    private UnitState _state = UnitState.Idle; 
    public UnitState State { get; }

    public void UpdateState(UnitState newState)
    {
        //Handle animations, other logic
        
        _state = newState;
    }
}
