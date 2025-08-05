using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private ShapeSettings shapeSettings;

    private float maxHealth;
    public bool isSquareSpecialAttacking = false;
    public int specialAttackDamageModifier;
    [SerializeField] private float currentHealth;

    public UnityEvent OnDeath; // Yo I discovered Unity events are pretty useful, check out the inspector
                               //WHAT THE FUCK ARE UNITY EVENTS


    public Slider healthBar;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    void Awake()
    {
        if (shapeSettings != null)
        {
            maxHealth = shapeSettings.defaultHealth;
        }
        else
        {
            Debug.LogWarning($"No ShapeSettings assigned to {gameObject.name}. Using default value.");
            maxHealth = 100f;
        }

        currentHealth = maxHealth;
        UpdateHealthBar();
    }


    public void UpdateHealthBar()
    {
        if (healthBar == null) return;
        healthBar.value = currentHealth / maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isSquareSpecialAttacking)
        {
            amount = amount / specialAttackDamageModifier;
        }
        currentHealth = Mathf.Max(currentHealth - amount, 0); // Don't let health go below 0
        UpdateHealthBar();
        if (currentHealth <= 0) Die();

    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
}
