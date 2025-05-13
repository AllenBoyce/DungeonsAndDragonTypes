using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    private RectTransform _healthBar;
    private TextMeshProUGUI _healthText;

    void Start()
    {
        _healthText.enabled = false;
    }

    public void Initialize(Unit unit) {
        maxHealth = unit.MaxHP;
        currentHealth = unit.CurrentHP;
        UpdateHealth(currentHealth);
    }

    public void UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        _healthText.text = currentHealth + "/" + maxHealth;
        float newWidth = (currentHealth / maxHealth) * _healthBar.rect.width;
        _healthBar.sizeDelta = new Vector2(newWidth, _healthBar.rect.height);
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
