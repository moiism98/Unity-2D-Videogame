using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI score;
    private PlayerController playerController;
    private string controllerInUse;

    [Header("Climb variables")]
    private Ladder ladder;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        // we suscribe the method to the gem event

        Gem.OnGemCollect += IncreseScore;

        score.text = "0";
    }
    
    private void Update()
    {
        // if we are close to a ladder in the level, we can show the corresponding controller button (based in which controller are we using right now)

        ladder?.ShowControllerButton(controllerInUse);
    }

    private void IncreseScore(int gemValue)
    {
        int currentScore = int.Parse(score.text);

        int newScore = currentScore + gemValue;

        score.text = newScore.ToString();
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
