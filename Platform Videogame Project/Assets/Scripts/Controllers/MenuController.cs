using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RetryGame()
    {
        // the scene loaded could change, in this case we are loading the current scene because it is the only one being used!

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 1f;
    }
}
