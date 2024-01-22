using UnityEngine;

public class Exit : MonoBehaviour
{
    public static bool nearExit = false;
    private DeviceController deviceController;
    private GameController gameController;
    [SerializeField] private GameObject action;
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    [SerializeField] private Animator animator;

    private void Start()
    {
        deviceController = FindObjectOfType<DeviceController>();

        gameController = FindObjectOfType<GameController>();

        action.SetActive(false);
    }
    
    private void Update()
    {
        deviceController.ShowDeviceUI(animator, animatorControllers);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(GameController.levelComplete)
            {
                action.SetActive(true);
                
                nearExit = true;

                gameController.SetGameAction(GameAction.exit);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            action.SetActive(false);

            nearExit = false;

            gameController.SetGameAction(GameAction.none);
        }
    }
    
}
