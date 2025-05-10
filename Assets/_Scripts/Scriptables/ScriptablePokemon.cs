using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Scriptable Objects/Pokemon")]
public class ScriptablePokemon : ScriptableObject
{
    public PokemonModel prefab;

    public Constants.PokemonSpecies species;
    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;

    public Sprite sprite;
    public Sprite portrait;
    public AudioClip moveSFX;
    public AudioClip cry;

    public List<ScriptableMove> learnableMoves;
    
    public string GetName() {
        return species.ToString();
    }
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