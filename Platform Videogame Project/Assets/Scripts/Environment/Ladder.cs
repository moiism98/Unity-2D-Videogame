using System;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private bool isClimbable = false;
    [SerializeField] private GameObject buttonBubble;
    [SerializeField] private Animator animator;
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    private GameController gameController;
    private PlayerController playerController;

    private void Start()
    {
        buttonBubble.SetActive(false);

        gameController = FindObjectOfType<GameController>();

        playerController = FindObjectOfType<PlayerController>();
    }
    private void OnTriggerEnter2D()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if(playerController != null)
            ShowClimbUI();
    }

    private void OnTriggerExit2D()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

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
