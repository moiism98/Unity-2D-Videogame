using UnityEngine;

public class Lever : MonoBehaviour
{
    private DeviceController deviceController;
    private GameController gameController;
    private Collider2D col;
    private Animator animator;
    [SerializeField] private Animator actionAnimator;
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    [SerializeField] private GameObject action;
    [SerializeField] private GameObject bridge;
    private void Start()
    {
        deviceController = FindObjectOfType<DeviceController>();

        gameController = FindObjectOfType<GameController>();

        col = GetComponent<Collider2D>();

        animator = GetComponent<Animator>();

        bridge.SetActive(false);

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

    public void Activate()
    {
        animator.SetTrigger("Activate");

        col.enabled = false;

        action.SetActive(false);

        bridge.SetActive(true);
    }
}
