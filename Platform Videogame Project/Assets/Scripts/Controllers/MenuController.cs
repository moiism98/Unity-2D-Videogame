using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    [SerializeField] private Animator animator; // action animator which shows what UI should show to the player (keyboard, xbox or ps UI)
    private DeviceController deviceController;
    private PlayerController playerController;
    
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        deviceController = FindObjectOfType<DeviceController>();
    }

    private void Update()
    {
        if(animator != null)
            deviceController.ShowDeviceUI(animator, animatorControllers);
    }

    public void PressToStartGame(InputAction.CallbackContext press)
    {
        if(press.performed)
            SceneManager.LoadScene("GameScene");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        GameController.isGamePaused = !GameController.isGamePaused;

        if(!GameController.isGamePaused)
            deviceController.UpdateGameActions("Player");
        else
            deviceController.UpdateGameActions("Menu");
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
