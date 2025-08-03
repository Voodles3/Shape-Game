using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject pauseUi;
    public GameObject deathUi;
    public GameObject winUi;
    public bool isPaused;
    public bool isDead;
    public bool hasWon;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isDead && !hasWon)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

    }

    public void DieCanvas()
    {
        if (!hasWon)
        {
            isDead = true;
            deathUi.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void WinCanvas()
    {
        if (!isDead)
        {
            hasWon = true;
            winUi.SetActive(true);
            Time.timeScale = 0;
        }
        
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pauseUi.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseUi.SetActive(false);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
