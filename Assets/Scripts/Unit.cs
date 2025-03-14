using UnityEngine;

public abstract Unit
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool _isMoving = false;
    private string state = "IDLE";
    private float _moveSpeed = 2.0f;

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

    public bool IsMoving
    {
        get { return _isMoving; }
        set { _isMoving = value; }
    }
}
