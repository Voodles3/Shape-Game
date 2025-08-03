using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject pauseUi;
    public GameObject deathUi;
    public bool isPaused;
    public bool isDead = false;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        isDead = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isDead == false)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
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
