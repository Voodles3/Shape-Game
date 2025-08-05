using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void Awake()
    {
        Time.timeScale = 1;
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void ContinueToMatch()
    {
        SceneManager.LoadScene(2);
    }

    public void SetPlayer(string name)
    {
        ShapeManager.Instance.SetPlayer(name);
        Invoke(nameof(ContinueToMatch), 0.5f);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
