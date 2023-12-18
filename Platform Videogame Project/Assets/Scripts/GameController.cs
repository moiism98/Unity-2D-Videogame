using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    private PlayerController playerController;
    private string controllerInUse;

    [Header("Climb variables")]
    private Ladder ladder;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }
    
    private void Update()
    {
        // if we are close to a ladder in the level, we can show the corresponding controller button (based in which controller are we using right now)

        if(ladder != null)
            ladder.ShowControllerButton(controllerInUse);
    }
    public void Climb()
    {
        // if we can climb a ladder

        if(ladder.GetIsClimbable())
        {
            // put the player on ladders position, we only take the X axis from the ladder because we have got the player's Y axis already.

            playerController.transform.position = new Vector2(ladder.transform.position.x, playerController.transform.position.y);

            // player start climb

            playerController.SetIsClimbing(true);
        }
    }

    public void SetControllerInUse(string controllerInUse)
    {
        this.controllerInUse = controllerInUse;
    }

    public void SetLadder(Ladder ladder)
    {
        this.ladder = ladder;
    }
}
