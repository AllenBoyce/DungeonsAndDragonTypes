using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Move")]
public class ScriptableMove : ScriptableObject
{
    [SerializeField] public string moveName;
    [SerializeField] public int power;
    [SerializeField] public TargetType targetType;
    [SerializeField] public int primaryRange;
    [SerializeField] public int secondaryRange = -1;
    [SerializeField] public TargetingUtility.Shape shape;
    [SerializeField] public MoveType moveType;
    [SerializeField] public string animationKey = "Attack"; //Later on this will change to keys
    [SerializeField] public int apCost;
    [SerializeField] public AudioClip sfx;
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

    public enum TargetType
    {
        Self,
        Sight,
        Any,
    }
    
}
