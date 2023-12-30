using System.Collections;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private GameObject earnedScore;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int scoreTextMaxLength = 17;
    private PlayerController playerController;
    private string controllerInUse;

    [Header("Climb variables")]
    private Ladder ladder;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        // we suscribe the method to the game events

        Gem.OnGemCollect += IncreseScore; 

        EnemyController.OnEnemyDie += IncreseScore;

        // this is when the game starts for the first time and not when there is a saved score! 

        scoreText.text = SetMaxScoreLength(scoreTextMaxLength);
    }
    
    private void Update()
    {
        // if we are close to a ladder in the level, we can show the corresponding controller button (based in which controller are we using right now)

        ladder?.ShowControllerButton(controllerInUse);
    }

    /// <summary>
    /// Set the score max digits to the game UI.
    /// </summary>
    /// <param name="maxLength"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    private string SetMaxScoreLength(int maxLength)
    {
        string score = "";

        for(int zeros = 0; zeros < maxLength; zeros++)
            score += "0";

        return score;
    }

    /// <summary>
    /// Triggers the coroutine that increase or set the game score.
    /// </summary>
    /// <param name="gemValue"></param>
    private void IncreseScore(int score)
    {
        StartCoroutine(SetNewScore(score)); 
    }
    
    /// <summary>
    /// Set the game score based on score points earned.
    /// </summary>
    /// <param name="gemValue"></param>
    /// <returns></returns>
    private IEnumerator SetNewScore(int score)
    {
        int newScore = int.Parse(scoreText.text) + score;
            
        string newScoreText = SetMaxScoreLength(scoreTextMaxLength - score.ToString().Length) + newScore.ToString();

        yield return new WaitForSeconds(.1f);

        scoreText.text = newScoreText;
    }

    public void ShowEarnedScore(int score, Transform scorePosition)
    {
        earnedScore.GetComponent<TextMeshPro>().text = "+ " + score.ToString();

        Instantiate(earnedScore, scorePosition.position, Quaternion.identity);
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
