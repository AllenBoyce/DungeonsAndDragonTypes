using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Move")]
public class ScriptableMove : ScriptableObject
{
    [SerializeField] public string moveName;
    [SerializeField] public int power;

    public enum MoveType
    {
        Normal,
        Grass,
        Water,
        Fire,
        Flying,
        Bug,
        Poison,
        Rock,
        Ground,
        Electric,
        Fighting,
        Ghost,
        Psychic,
        Ice,
        Dragon,
        Dark,
        Steel,
        Fairy
    }
}
