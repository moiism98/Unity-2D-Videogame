using System;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private ClimbPoint climbPoint = ClimbPoint.bottom;
    [SerializeField] public static bool isClimbable = false;
    [SerializeField] private GameObject buttonBubble;
    [SerializeField] private Animator animator;
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    private GameController gameController;
    private PlayerController playerController;
    private DeviceController deviceController;
    private Collider2D climbPointCollider;

    private void Start()
    {
        buttonBubble.SetActive(false);

        gameController = FindObjectOfType<GameController>();

        playerController = FindObjectOfType<PlayerController>();

        deviceController = FindObjectOfType<DeviceController>();

        climbPointCollider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(playerController.GetIsClimbing()) // if we touch the climb point when we are climbing
            {
                // we check which point we reach, and then move the player to it's position

                if(climbPoint.Equals(ClimbPoint.top))
                    playerController.gameObject.transform.position = new Vector2(climbPointCollider.transform.position.x, climbPointCollider.transform.position.y + climbPointCollider.offset.y);
                else
                    playerController.gameObject.transform.position = climbPointCollider.transform.position;
                    
                // also the player stops climbing

                playerController.SetIsClimbing(false);

                playerController.GetComponent<Animator>().SetBool("Climbing", false);

                // and set the movement to zero avoiding the player keep moving with the climb movement velocity

                playerController.SetMovement(0f);
            }
            else
                ShowClimbUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {       
        if(collision.CompareTag("Player"))
            HideClimbUI();
    }

    private void ShowClimbUI()
    {
        // we can climb

        isClimbable = true;

        // set this ladder on GameController and PlayerController so they can manage the climb action

        gameController.SetLadder(this);

        gameController.SetGameAction(GameAction.climb);
    }

    private void HideClimbUI()
    {
        // we can not climb, we are far from the ladder

        isClimbable = false;

        // hide the climb UI and rebind the animators animation

        HideControllerButton();

        // set GameControllers ladder to null, to reset the action

        gameController.SetLadder(null);

        gameController.SetGameAction(GameAction.none);
    }

    public void ShowControllerButton()
    {
        deviceController.ShowDeviceUI(animator, animatorControllers);

        buttonBubble.SetActive(true);
    }

    public void HideControllerButton()
    {
        buttonBubble.SetActive(false);
        
        animator.Rebind();
    }

    public bool GetIsClimbable()
    {
        return isClimbable;
    }
}
