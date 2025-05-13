using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    private TextMeshProUGUI _healthText;

    [SerializeField] private List<Sprite> _healthBarSprites;
    [SerializeField] private SpriteRenderer _healthBarRenderer;

    void Start()
    {
        //_healthText.enabled = false;
    }

    public void Initialize(Unit unit) {
        Debug.Log("HealthBar Initialize: " + unit.GetType());
        maxHealth = unit.MaxHP;
        currentHealth = unit.CurrentHP;
        UpdateHealth(currentHealth);
    }

    public void UpdateHealth(int newHealth)
    {
        Debug.Log("HealthBar UpdateHealth: " + newHealth);
        int maxIndex = _healthBarSprites.Count - 1;
        float healthRatio = (float)(newHealth / (float)maxHealth);
        Debug.Log("HealthBar healthRatio: " + healthRatio);
        int currentIndex = (int) (healthRatio * maxIndex);
        Debug.Log("HealthBar currentIndex: " + currentIndex);
        _healthBarRenderer.sprite = _healthBarSprites[currentIndex];
    }

    public void UpdateHealth(Unit unit) {
        maxHealth = unit.MaxHP;
        UpdateHealth(unit.CurrentHP);
    }

    void OnMouseOver()
    {
        _healthText.enabled = true;
    }
    void OnMouseExit()
    {
        _healthText.enabled = false;
    }
}
