using UnityEngine;
using UnityEngine.SceneManagement;

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager Instance { get; private set; }

    [SerializeField] private GameObject circlePlayer;
    [SerializeField] private GameObject squarePlayer;
    [SerializeField] private GameObject trianglePlayer;

    public GameObject activePlayer;
    public bool playerSpawned = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2 && !playerSpawned)
        {
            Instantiate(activePlayer);
            playerSpawned = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            playerSpawned = false;
        }
    }

    public void SetPlayer(string playerName)
    {
        if (playerName == "circle")
        {
            activePlayer = circlePlayer;
        }
        else if (playerName == "square")
        {
            activePlayer = squarePlayer;

        }
        else if (playerName == "triangle")
        {
            activePlayer = trianglePlayer;
        }
    }
}
