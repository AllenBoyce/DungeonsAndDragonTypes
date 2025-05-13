using UnityEngine;

public class UnitShadow : MonoBehaviour
{
    [SerializeField] private Sprite player1Shadow;
    [SerializeField] private Sprite player2Shadow;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    void Start()
    {
    }

    public void SetShadow(int player) {
        Debug.Log("Setting shadow for player: " + player);
        Debug.Log("Shadow sprite: " + _spriteRenderer);
        if(player == 0) {
            _spriteRenderer.sprite = player1Shadow;
        } else {
            _spriteRenderer.sprite = player2Shadow;
        }
    }
    void Update()
    {
        
    }
}
