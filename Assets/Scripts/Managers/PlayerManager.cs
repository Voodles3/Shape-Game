using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public Transform ActivePlayerTransform { get; private set; }

    private ShapeAttack playerAttack;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterActivePlayer(Transform playerTransform)
    {
        ActivePlayerTransform = playerTransform;
        playerAttack = playerTransform.GetComponent<ShapeAttack>();
    }

    public void UnregisterActivePlayer()
    {
        ActivePlayerTransform = null;
    }

    public bool IsPlayerAttacking => playerAttack != null && playerAttack.isAttacking;
}
