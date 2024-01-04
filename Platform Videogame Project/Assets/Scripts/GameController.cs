using System.Collections;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private GameObject earnedScore;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int scoreTextMaxLength = 17;
    [SerializeField] GameObject arrow;

        [Header("Player Health")]
        [SerializeField] private GameObject healthContainer;
        [SerializeField] private GameObject heart;
        [SerializeField] private GameObject emptyHeart;
    public static bool isArrowReady = false;
    private PlayerController playerController;
    private string controllerInUse;

    [Header("Climb variables")]
    private Ladder ladder;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        SetPlayerHealth();

        // we suscribe the method to the game events

        Gem.OnGemCollect += IncreseScore;

        Heart.OnHeartCollect += HealPlayer;

        EnemyController.OnEnemyDie += IncreseScore;

        // this is when the game starts for the first time and not when there is a saved score! 

        scoreText.text = SetMaxScoreLength(scoreTextMaxLength);
    }
    
    private void Update()
    {
        // if we are close to a ladder in the level, we can show the corresponding controller button (based in which controller are we using right now)

        ladder?.ShowControllerButton(controllerInUse);

        arrow.SetActive(isArrowReady);
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
    
    /// <summary>
    /// Display the points player earn.
    /// </summary>
    /// <param name="score"></param>
    /// <param name="scorePosition"></param> <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    /// <param name="scorePosition"></param>
    public void ShowEarnedScore(int score, Transform scorePosition)
    {
        earnedScore.GetComponent<TextMeshPro>().text = "+ " + score.ToString();

        Instantiate(earnedScore, scorePosition.position, Quaternion.identity);
    }

    /// <summary>
    /// Set the initial player's health based on his maximum health.
    /// </summary>
    /// <param name="controllerInUse"></param>
    private void SetPlayerHealth()
    {
        for(int hearts = 0; hearts < playerController.GetMaxHealth(); hearts++)
        {
            Instantiate(heart, healthContainer.transform);
        }
    }

    /// <summary>
    /// Update the player's UI health based on the player's current health.
    /// </summary>
    /// <param name="health"></param> <summary>
    /// 
    /// </summary>
    /// <param name="health"></param>
    public void UpdatePlayerHealth(int health)
    {
        if(health < playerController.GetMaxHealth()) // 2 <= 3
        {
            UIHeart[] hearts = healthContainer.GetComponentsInChildren<UIHeart>();

            UIHeart heart = hearts[health]; // if heart is 2, we'll take the last array's heart

            if(heart.GetHeartType().Equals(HeartType.heart)) // if the heart is the regular one will replace it for the empty one (player got hurted)
                heart.SetHeartType(HeartType.emptyHeart);
            else // if not, will replace it with the regular one (player is now healing with a heart item)
                heart.SetHeartType(HeartType.heart);
        }
    }

    /// <summary>
    /// Heals the player and change the health UI.
    /// </summary>
    /// <param name="health"></param> <summary>
    /// 
    /// </summary>
    /// <param name="health"></param>
    private void HealPlayer(int health)
    {
        UpdatePlayerHealth(playerController.GetHealth()); // change the UI

        playerController.SetHealth(playerController.GetHealth() + health); // and change the player's health value adding 1 health point.

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
