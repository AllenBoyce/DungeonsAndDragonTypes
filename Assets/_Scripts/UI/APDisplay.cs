using UnityEngine;
using UnityEngine.UI;

public class APDisplay : MonoBehaviour
{
    [SerializeField] private Sprite _emptyAP;
    [SerializeField] private Sprite _tempAP;
    [SerializeField] private Sprite _fullAP;
    private SpriteRenderer _spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        SetAP(2);
    }

    // 0: empty, 1: temp, 2: full
    public void SetAP(int displayState)
    {
        switch (displayState)
        {
            case 0:
                _spriteRenderer.sprite = _emptyAP;
                break;
            case 1:
                _spriteRenderer.sprite = _tempAP;
                break;
            case 2:
                _spriteRenderer.sprite = _fullAP;
                break;
            default:
                _spriteRenderer.sprite = _emptyAP;
                break;
        }
    }
}
