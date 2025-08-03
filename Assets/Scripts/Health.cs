using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private ShapeSettings shapeSettings;

    private float maxHealth;
    [SerializeField] private float currentHealth;

    public UnityEvent OnDeath; // Yo I discovered Unity events are pretty useful, check out the inspector

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
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0); // Don't let health go below 0
        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }

    public void Revive()
    {
        currentHealth = maxHealth;
    }
}
