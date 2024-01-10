using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private PlayerController playerController;
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        GameController.isGamePaused = !GameController.isGamePaused;

        // enable again the player input if not the player can not execute his actions.

        PlayerInput playerInput = playerController.GetPlayerInput();

        playerInput.enabled = !GameController.isGamePaused;
    }
    public void RetryGame()
    {
        // the scene loaded could change, in this case we are loading the current scene because it is the only one being used!

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // we reset the game states

        Time.timeScale = 1f;

        GameController.isGameOver = !GameController.isGameOver;
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
