using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private ParticleSystem bloodSplash;
    [SerializeField] private Slider healthBar;
    [SerializeField] private ShapeSettings shapeSettings;
    [SerializeField] private UnityEvent OnDeath; // Yo I discovered Unity events are pretty useful, check out the inspector
                                                 //WHAT THE FUCK ARE UNITY EVENTS
    [SerializeField] private float currentHealth;

    private float maxHealth;
    private float currentDamageMultiplier = 1f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;


    void Awake()
    {
        LoadSettings();

        currentHealth = maxHealth;
        UpdateHealthBar();
    }


    public void UpdateHealthBar()
    {
        if (healthBar == null) return;
        healthBar.value = currentHealth / maxHealth;
    }

    public void TakeDamage(float amount)
    {
        amount *= currentDamageMultiplier;

        currentHealth = Mathf.Max(currentHealth - amount, 0); // Don't let health go below 0
        UpdateHealthBar();
        bloodSplash.Play();
        if (currentHealth <= 0) Die();

    }

    public void AddDamageMultiplier(float amount) => currentDamageMultiplier *= amount;
    public void RemoveDamageMultiplier(float amount) => currentDamageMultiplier = Mathf.Max(currentDamageMultiplier / amount, 1f);
    public void ResetDamageMultiplier() => currentDamageMultiplier = 1f;

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

    private void LoadSettings()
    {
        if (shapeSettings == null)
        {
            Debug.LogWarning($"No ShapeSettings assigned to {gameObject.name}. Using a default value.");
            maxHealth = 100f;
            return;
        }

        maxHealth = shapeSettings.defaultMaxHealth;
    }
}
