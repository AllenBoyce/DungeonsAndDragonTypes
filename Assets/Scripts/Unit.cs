using UnityEngine;

public class Unit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool _isMoving = false;
    private string state = "IDLE";
    private float _moveSpeed = 2.0f;

    void Start()
    {
        
    }

    public string State
    {
        get { return state; }
        set { state = value; }
    }

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
