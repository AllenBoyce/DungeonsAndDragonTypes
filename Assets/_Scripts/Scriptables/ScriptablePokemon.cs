using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Scriptable Objects/Pokemon")]
public class ScriptablePokemon : ScriptableObject
{
    public PokemonModel prefab;

    public string pokemonName;
    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;

    public Sprite sprite;
    public Sprite portrait;

    public List<ScriptableMove> learnableMoves;
    
    
}

[Serializable]
public struct Stats
{
    public int hp;
    public int atk;
    public int def;
    public int spatk;
    public int spdef;
    public int speed;
    public int moveSpeed;
}