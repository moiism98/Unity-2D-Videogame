using UnityEngine;

public class Lever : MonoBehaviour
{
    private DeviceController deviceController;
    private GameController gameController;
    private AudioManager audioManager;
    private Collider2D col;
    private Animator animator;
    [SerializeField] private Animator actionAnimator;
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    [SerializeField] private GameObject action;
    [SerializeField] private GameObject bridge;
    [SerializeField] private bool visible = false;
    private void Start()
    {
        deviceController = FindObjectOfType<DeviceController>();

        gameController = FindObjectOfType<GameController>();

        audioManager = FindObjectOfType<AudioManager>();

        col = GetComponent<Collider2D>();

        animator = GetComponent<Animator>();

        bridge.SetActive(visible);

        action.SetActive(false);
    }

    private void Update()
    {
        deviceController.ShowDeviceUI(actionAnimator, animatorControllers);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            action.SetActive(true);

            gameController.SetGameAction(GameAction.lever);

            deviceController.SetLever(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            action.SetActive(false);

            gameController.SetGameAction(GameAction.none);

            deviceController.SetLever(null);
        }
    }

    /// <summary>
    /// Triggers the lever which is going to display or hide his attached bridge.
    /// </summary>
    public void Activate()
    {
        animator.SetTrigger("Activate");

        audioManager.PlaySound("Lever");

        col.enabled = false;

        action.SetActive(false);

        bridge.SetActive(!visible);
    }
}
