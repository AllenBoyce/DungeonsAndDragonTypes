using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Pokemon
{
    private string name;
    private Dictionary<string, int> baseStats;
    private Dictionary<string, int> currentStats;

    public Pokemon(string name)
    {
        this.name = name;
        
    }
}
