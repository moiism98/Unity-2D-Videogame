using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter2D()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if(playerController != null)
        {
            // display climb button UI

            
        }
    }

     private void OnTriggerExit2D()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if(playerController != null)
        {
            // hide climb button UI


        }
    }
}
