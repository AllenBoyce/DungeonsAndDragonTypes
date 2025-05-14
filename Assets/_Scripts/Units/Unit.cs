using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    private Transform _transform;
    
    private int _currentHP;
    private int _maxHP;
    private int _currentAP;
    private int _maxAP;
    private Vector2Int _gridPosition;
    [SerializeField]private int _playerOwner;
    private UnitShadow _shadow;
    private HealthBar _healthBar;
    [SerializeField] private ScriptablePokemon pokemonData;
    private List<ScriptableMove> _learnedMoves;
    private PokemonModel _model;

    private Direction _currentDirection;
    private Material _greyscaleMaterial;
    public void Start()
    {
        GameManager.OnUnitHurt += OnUnitHurt;
        _greyscaleMaterial = Resources.Load<Material>("Materials/GreyscaleMat");
    }

    #region Getters and Setters and other functions
    public int CurrentHP { get { return _currentHP; } }
    public int MaxHP { get { return _maxHP; } }
    public int CurrentAP { get { return _currentAP; } }
    public int MaxAP { get { return _maxAP; } }
    public void SetCurrentAP(int ap) { _currentAP = ap; }
    public ScriptablePokemon PokemonData { get { return pokemonData; } }
    public void ConsumeAP(int ap) { _currentAP = Math.Max(0, _currentAP - ap); }
    public List<ScriptableMove> GetLearnedMoves() { return _learnedMoves;}
    public Direction GetCurrentDirection() { return _currentDirection; }
    public void SetCurrentDirection(Direction direction) { _currentDirection = direction; }
    #endregion
    public void Initialize(ScriptablePokemon pokemon, Vector3 position, int player)
    {
        Debug.Log("Unit Initialize: " + pokemon.GetName() + " for player " + player);
        pokemonData = pokemon;
        _transform = transform;
        //Hiding model behind camera
        //Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, -500);
        _transform.position = position;
        _model = Instantiate(pokemon.prefab, position, Quaternion.identity, transform);
        Debug.Log("Unit Model: " + _model.name);
        _model.name = $"{pokemon.GetName()}_Model";
        _maxHP = pokemonData.BaseStats.hp;
        _currentHP = _maxHP;
        _maxAP = 5; //TEMPORARILY HARDCODING MAX AP TO 5 FOR ALL MONS. FOR NOW.
        _currentAP = _maxAP;
        _playerOwner = player;
        
        //Temporary:
        _learnedMoves = pokemonData.learnableMoves;
        GameObject shadow = _model.transform.Find("ColorShadow").gameObject;
        _shadow = shadow.GetComponent<UnitShadow>();
        Debug.Log("Unit Shadow: " + _shadow.GetType());
        _shadow.SetShadow(player);
        GameObject healthBar = _model.transform.Find("HealthBar").gameObject;
        _healthBar = healthBar.GetComponent<HealthBar>();
        Debug.Log("Unit HealthBar: " + _healthBar.GetType());
        _healthBar.Initialize(this);
        Debug.Log("Unit HealthBar Initialized");
    }
    
    public void SetShadow(int player) {
        _shadow.SetShadow(player);
    }
    

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
    public int PlayerOwner { get { return _playerOwner; } }

    //Temp: loop has no effect
    public void PlayAnimation(string animationName, Direction direction, bool loop = true)
    {
        _currentDirection = direction;
        _model.PlayAnimation(animationName, direction);
    }
    public void PlayAnimation(string animationName, bool loop = true)
    {
        Debug.Log("Playing animation: " + animationName);
        _model.PlayAnimation(animationName, _currentDirection);
    }

    public async Task PlayAnimationAsync(string animationName)
    {
        Debug.Log($"Playing async animation: {animationName}");
        PlayAnimation(animationName, _currentDirection);
        
        // Get the animation clip length
        float clipLength = GetAnimationClipLength(animationName, _currentDirection);
        int waitTime = Mathf.RoundToInt(clipLength * 1000); // Convert to milliseconds
        
        // Wait for the animation to complete
        await Task.Delay(waitTime);
        
        Debug.Log($"Animation {animationName} completed");
    }
    
    public async Task PlayAnimationAsync(string animationName, Direction direction)
    {
        float waitModifier = 0f;
        _currentDirection = direction;
        Debug.Log($"Playing async animation: {animationName} in direction: {direction}");
        PlayAnimation(animationName, direction);
        
        // Get the animation clip length
        float clipLength = GetAnimationClipLength(animationName, direction);
        int waitTime = Mathf.RoundToInt(clipLength * 1000); // Convert to milliseconds
        
        // Wait for the animation to complete
        await Task.Delay(Mathf.RoundToInt(waitTime * waitModifier));
        
        Debug.Log($"Animation {animationName} in direction {direction} completed");
    }
    
    private float GetAnimationClipLength(string animationName, Direction direction)
    {
        Animator animator = _model.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found, using default animation length");
            return 1.0f; // Default 1 second
        }
        
        // Get the runtime animator controller
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        if (controller == null)
        {
            Debug.LogWarning("RuntimeAnimatorController not found, using default animation length");
            return 1.0f;
        }
        
        // Find the animation clip with the expected name format: PokemonName_AnimationName_Direction
        string clipName = $"{pokemonData.name}_{animationName}_{direction.ToString()}";
        
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        
        // Clip not found, use default
        Debug.LogWarning($"Animation clip '{clipName}' not found, using default animation length");
        return 1.0f;
    }

    public void OnUnitHurt(Unit unit, ScriptableMove move, Vector2Int originTile)
    {
        if(unit == this)
        {
            Hurt(move.power);
        }
    }

    public void Hurt(int damage)
    {
        //_state = UnitState.Hurting;
        //PlayAnimation("Hurt");
        takeDamage(damage);
    }

    private void takeDamage(int damage)
    {
        Debug.Log("Unit takeDamage: " + damage);
        _currentHP = Math.Max(0, _currentHP - damage);
        Debug.Log("Unit currentHP: " + _currentHP);
        _healthBar.UpdateHealth(this);
        Debug.Log("Unit healthBar updated");
        if (_currentHP <= 0)
        {
            Faint();
        }
    }

    public void Faint()
    {
        //The sleep animation only has south direction, so we need to set the direction to south
        _currentDirection = Direction.South;
        PlayAnimation("Sleep");
        SpriteRenderer renderer = _model.GetComponent<SpriteRenderer>();
        Material instanceMaterial = new Material(_greyscaleMaterial);
        instanceMaterial.mainTexture = renderer.material.mainTexture;
        renderer.material = instanceMaterial;

        _state = UnitState.Fainted;
        GameManager.Instance.UnhandledFaint = true;
        Debug.Log("Unit Fainted");
        
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
    public UnitState State { get { return _state; } }

    public void UpdateState(UnitState newState)
    {
        //Handle animations, other logic
        
        _state = newState;
    }
}
