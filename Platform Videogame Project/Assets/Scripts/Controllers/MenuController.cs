using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // this properties are publics and hidden on inspector because we are using a custom editor for this class.
    [HideInInspector] public Scene scene = Scene.main;
    [HideInInspector] public RuntimeAnimatorController[] animatorControllers;
    [HideInInspector] public Animator animator; // action animator which shows what UI should show to the player (keyboard, xbox or ps UI)
    private GameController gameController;
    private DeviceController deviceController;
    private PlayerController playerController;
    
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

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
        {
            SceneManager.LoadScene("GameScene");
        
            Time.timeScale = 0.0f;
        }
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
        gameController.Retry();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
