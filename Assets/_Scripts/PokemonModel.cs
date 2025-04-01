using System;
using UnityEngine;

public class PokemonModel : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private string pokemonName;
    private string _animationFolderPath;
    private Transform _transform;

    #region Animation Testing Varaibles
    private string[] _directions = { "North", "South", "East", "West" };
    private string[] _animationNames = { "Idle", "Attack", "Hurt", "Walk" };
    private int _directionIdx = 0;
    private int _animationIdx = 0;
    #endregion
    
    /**
     * Sets up the components of this PokemonModel and beings the Idle-South animation
     */
    void Start()
    {
        animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _animationFolderPath = $"Pokemon/Sprites/Pokemon/{pokemonName}";
        
        if(animator == null)
        {
            Debug.LogError($"No Animator component found on Pokémon {pokemonName}!");
            return;
        }
        
        if(spriteRenderer == null)
        {
            Debug.LogError($"No SpriteRenderer component found on Pokémon {pokemonName}!");
            return;
        }
        PlayAnimation("Idle", Unit.Direction.South);
    }
    
    public void PlayAnimation(string animName, Unit.Direction direction)
    {
        if (animator != null)
        {
            animator.Play($"{pokemonName}_{animName}_{direction.ToString()}");
        }
    }

    /*//CURRENT ISSUE: RIGID BODY IS BEING GENERATED BEFORE ANIMATION SPRITES SO RIGIDBODY HITBOX IS SUPER THIN
    //MAKE NEW RIGIDBODY IN RUNTIME AFTER FIRST FRAME OF ANIMATION IS RENDERED?
    //TEMPORARY SOLUTION IN ACTION: MANUAL RIGIDBODY DIMENSIONS ESTABLISHED IN PREFAB
    void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
        PlayAnimation(_animationNames[_animationIdx], _directions[_directionIdx]);
        _directionIdx = (_directionIdx + 1) % _directions.Length;
        _animationIdx = (_animationIdx + 1) % _animationNames.Length;
    }*/
}
