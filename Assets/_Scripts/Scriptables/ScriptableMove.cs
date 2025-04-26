using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Move")]
public class ScriptableMove : ScriptableObject
{
    [SerializeField] public string moveName;
    [SerializeField] public int power;
    [SerializeField] public OriginType originType;
    [SerializeField] public int primaryRange;
    [SerializeField] public int secondaryRange = -1;
    [SerializeField] public TargetingUtility.Shape shape;
    [SerializeField] public MoveType moveType;
    [SerializeField] public string animationKey = "Attack"; //Later on this will change to keys
    [SerializeField] public int apCost;
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

    public enum OriginType
    {
        Self,
        Sight,
        Any,
    }
    
}
