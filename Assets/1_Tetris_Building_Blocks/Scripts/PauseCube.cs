using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubePause : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;


    public void Pause()
    {
        // Set the time scale to 0 to pause the game
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;

    }
      

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;

      
    }

    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("1-Scene");
    }
}

