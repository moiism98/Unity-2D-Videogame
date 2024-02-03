using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // this properties are publics and hidden on inspector because we are using a custom editor for this class.
    [HideInInspector] public Scene scene = Scene.main;
    [HideInInspector] public RuntimeAnimatorController[] animatorControllers;
    [HideInInspector] public Animator animator; // action animator which shows what UI should show to the player (keyboard, xbox or ps UI)
    [HideInInspector] public GameObject action;
    [HideInInspector] public GameObject menu;
    [HideInInspector] public GameObject controlsMenu;

    private GameController gameController;
    private DeviceController deviceController;
    private AudioManager audioManager;
    
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        deviceController = FindObjectOfType<DeviceController>();

        audioManager = FindObjectOfType<AudioManager>();

        if(scene.Equals(Scene.main))
        {
            menu.SetActive(false);

            controlsMenu.SetActive(false);

            audioManager.PlaySound("Main Menu");
        }
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
            // show play and controls menu

            action.SetActive(false);

            menu.SetActive(true);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
        
        Time.timeScale = 0.0f;
    }

    public void ControlsMenu()
    {
        if(controlsMenu.activeSelf)
        {
            controlsMenu.SetActive(false);

            menu.SetActive(true);
        }
        else
        {
            controlsMenu.SetActive(true);

            menu.SetActive(false);
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
