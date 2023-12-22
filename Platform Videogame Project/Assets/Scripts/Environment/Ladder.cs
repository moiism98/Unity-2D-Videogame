using System;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private ClimbPoint climbPoint = ClimbPoint.bottom;
    [SerializeField] private bool isClimbable = false;
    [SerializeField] private GameObject buttonBubble;
    [SerializeField] private Animator animator;
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    private GameController gameController;
    private PlayerController playerController;
    private Collider2D climbPointCollider;

    private void Start()
    {
        buttonBubble.SetActive(false);

        gameController = FindObjectOfType<GameController>();

        playerController = FindObjectOfType<PlayerController>();

        climbPointCollider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D()
    {
        if(playerController != null)
        {
            if(playerController.GetIsClimbing()) // if we touch the climb point when we are climbing
            {
                // we check which point we reach, and then move the player to it's position

                if(climbPoint.Equals(ClimbPoint.top))
                    playerController.gameObject.transform.position = new Vector2(climbPointCollider.transform.position.x, climbPointCollider.transform.position.y + climbPointCollider.offset.y);
                else
                    playerController.gameObject.transform.position = climbPointCollider.transform.position;
                    
                // also the player stops climbing

                playerController.SetIsClimbing(!playerController.GetIsClimbing());

                playerController.GetComponent<Animator>().SetBool("Climbing", playerController.GetIsClimbing());

                // and the set the movement to zero avoiding the player keep moving with the climb movement velocity

                playerController.SetMovement(0f);
            }
            else
                ShowClimbUI();
        }
    }

    private void OnTriggerExit2D()
    {
        if(playerController != null)
            HideClimbUI();
    }

    private void ShowClimbUI()
    {
        // we can climb

        isClimbable = true;

        // set this ladder on GameController and PlayerController so they can manage the climb action

        gameController.SetLadder(this);

        playerController.SetLadder(this);
    }

    private void HideClimbUI()
    {
        // we can not climb, we are far from the ladder

        isClimbable = false;

        // hide the climb UI and rebind the animators animation

        HideControllerButton();

        // set GameControllers ladder to null, to reset the action

        gameController.SetLadder(null);
    }

    public void ShowControllerButton(string controllerInUse)
    {
        //RuntimeAnimatorController animatorController;

        // this method it's going to determinate which controller are we using, it helps GameController to show the corresponding UI button.

        // every frame the GameController knows what controller are we using and this method display the correct animation!

        switch(controllerInUse)
        {
            case "Keyboard": GetAnimatorController("keyboard"); break;

            case "Xbox Controller": GetAnimatorController("xbox"); break;

            default: GetAnimatorController("ps"); break;
        }

        buttonBubble.SetActive(true);
    }

    public void HideControllerButton()
    {
        buttonBubble.SetActive(false);
        
        animator.Rebind();
    }

    private void GetAnimatorController(string controllerName)
    {
        RuntimeAnimatorController animatorController = Array.Find(animatorControllers, controller => controller.name.Contains(controllerName));

        animator.runtimeAnimatorController = animatorController;
    }

    public bool GetIsClimbable()
    {
        return isClimbable;
    }
}
