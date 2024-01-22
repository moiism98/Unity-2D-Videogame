using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{

    [Header("Level")]
    [SerializeField] private GameAction action = GameAction.none;
    [HideInInspector] public GameMode gameMode = GameMode.regular;
    public static bool levelComplete = false;
    [SerializeField] private Level[] levels;
    private string levelName = "Beach Forest"; // this is NOT going to be static, but for now we only have 1 level.
    private Level selectedLevel;
    private Stage currentStage;

    [Header("Coroutine states")]
    private bool isHealing = false;
    private bool isCollectingItem = false;

    [Header("Game UI")]
    [SerializeField] private GameObject gameUIPanel;
    [SerializeField] private GameObject earnedScore;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] GameObject arrow;

        [Header("Transition")]
        [SerializeField] private GameObject transitionScreen;
        [SerializeField] private GameObject bonusTransitionScreen;
        [SerializeField] private float transitionTime = 5f;
        [SerializeField] private TextMeshProUGUI transitionScore;
        [SerializeField] private TextMeshProUGUI transitionLifes;
        [HideInInspector] public TextMeshProUGUI currentStageText;
        [HideInInspector] public TextMeshProUGUI lastStageText;
        private bool transitionLoaded = false;

        [Header("Pause UI")]
        [SerializeField] GameObject pauseScreen;
        [SerializeField] private Button pausedSelectedButton;
        public static bool isGamePaused = false;

        [Header("Game Over UI")]
        [SerializeField] GameObject gameOverScreen;
        public static bool isGameOver = false;


        [Header("Player Health")]
        [SerializeField] private GameObject healthContainer;
        [SerializeField] private GameObject heart;

        [Header("Player Lifes")]
        [SerializeField] private TextMeshProUGUI lifesText;
        [SerializeField] private int playerLifes = 0;
        [SerializeField] private int fruits4lifes = 3;
        [SerializeField] private int fruits = 0;

        [Header("Keys")]
        [HideInInspector] public TextMeshProUGUI keysText;
        public static int keysCollected = 0;
        public static int keysToCollect = 5; // 5 keys
    public static bool isArrowReady = false;

    [Header("Controllers")]
    private PlayerController playerController;
    private DeviceController deviceController;

    [Header("Climb variables")]
    private Ladder ladder;
    private static GameController instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        DontDestroyOnLoad(gameUIPanel);
    }

    private void Start()
    {
        LoadInitialLevel();

        Invoke("StopTransition", transitionTime);

        gameUIPanel.SetActive(false);

        transitionScreen.SetActive(true);

        bonusTransitionScreen.SetActive(false);

        gameOverScreen.SetActive(false);

        pauseScreen.SetActive(isGamePaused);

        playerController = FindObjectOfType<PlayerController>();

        deviceController = FindObjectOfType<DeviceController>();

        SetPlayerHealth();

        // we suscribe the method to the game events

        Key.OnKeyCollect += CollectKey;

        Gem.OnGemCollect += IncreseScore;

        Heart.OnHeartCollect += HealPlayer;

        Fruit.OnFruitCollect += CollectFruit;

        EnemyController.OnEnemyDie += IncreseScore;

        // this is when the game starts for the first time and not when there is a saved values! 

        scoreText.text = "0";

        lifesText.text = playerLifes.ToString();

        DisableEnemyHitPoint();
    }
    
    private void Update()
    {
        // if we are close to a ladder in the level, we can show the corresponding controller button (based in which controller are we using right now)

        ladder?.ShowControllerButton();

        arrow.SetActive(isArrowReady);

        FruitsForLifes();

        lifesText.text = playerLifes.ToString();

        keysText.text = keysCollected.ToString();

        transitionScore.text = scoreText.text;

        transitionLifes.text = playerLifes.ToString();

        if(gameMode.Equals(GameMode.bonus) && !transitionLoaded)
        {
            transitionLoaded = true;

            bonusTransitionScreen.SetActive(true);

            Invoke("StopTransition", transitionTime);
        }
    }

    private void LoadInitialLevel()
    {
        selectedLevel = Array.Find(levels, level => level.GetName().Equals(levelName));

        if(selectedLevel != null)
        {
            lastStageText.text = selectedLevel.GetStages().Count.ToString();

            currentStage = selectedLevel.GetStages()[0];

            currentStageText.text = currentStage.GetIndex().ToString();
        }
    }

    public void LoadNextLevel()
    {
        switch(gameMode)
        {
            case GameMode.regular:

                if(levelComplete && Exit.nearExit)
                {
                    gameMode = GameMode.bonus;

                    SceneManager.LoadScene("BonusScene");

                    Time.timeScale = 0f;
                }

            break;
        }
        
    }

    private void StopTransition()
    {
        Time.timeScale = 1;            
        
        switch(gameMode)
        {
            case GameMode.regular: transitionScreen.SetActive(false); break;

            case GameMode.bonus: 

                transitionLoaded = true;
            
                bonusTransitionScreen.SetActive(false); 
            
            break;
        }

        gameUIPanel.SetActive(true);
    }

    public void Pause(InputAction.CallbackContext pause)
    {
        if(!isGameOver)
        {
            if(pause.performed)
                ShowPauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        isGamePaused = !isGamePaused;

        if(isGamePaused)
        {
            pauseScreen.SetActive(true);

            pausedSelectedButton.Select(); // every time we pause the game put this button as first button selected!

            Time.timeScale = 0f;

            deviceController.UpdateGameActions("Menu");
        }
        else 
        {
            pauseScreen.SetActive(false);

            Time.timeScale = 1f;

            deviceController.UpdateGameActions("Player");
        }
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

        yield return new WaitForSeconds(.1f);

        scoreText.text = newScore.ToString();
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
            
            if(health >= 0)
            {
                UIHeart heart = hearts[health]; // if heart is 2, we'll take the last array's heart

                if(heart.GetHeartType().Equals(HeartType.heart)) // if the heart is the regular one will replace it for the empty one (player got hurted)
                    heart.SetHeartType(HeartType.emptyHeart);
                else // if not, will replace it with the regular one (player is now healing with a heart item)
                    heart.SetHeartType(HeartType.heart);
            }
        }
    }

    /// <summary>
    /// Heals the player and change the health UI.
    /// </summary>
    /// <param name="health"></param> <summary>
    /// 
    /// </summary>
    /// <param name="health"></param>
    private void HealPlayer(int health) // this method has to call a coroutine, player is healing sometimes twice with a single heart!
    {
        StartCoroutine(HealCoroutine(health));
    }

    private IEnumerator HealCoroutine(int health)
    {
        if(!isHealing)
        {
            isHealing = true;

            UpdatePlayerHealth(playerController.GetHealth()); // change the UI

            playerController.SetHealth(playerController.GetHealth() + health); // and change the player's health value adding 1 health point.

            yield return new WaitForSeconds(.15f);

            isHealing = false;
        }
    }

    /// <summary>
    /// Add 1 on fruits collected.
    /// </summary>
    /// <param name="fruitScore"></param> <summary>
    /// 
    /// </summary>
    /// <param name="fruitScore"></param>
    private void CollectFruit(int fruitScore)
    {
        StartCoroutine(AddFruit(fruitScore));
    }

    private IEnumerator AddFruit(int fruitScore)
    {
        if(!isCollectingItem)
        {
            isCollectingItem = true;

            fruits++;

            IncreseScore(fruitScore);

            yield return new WaitForSeconds(.15f);

            isCollectingItem = false;

        }
    }

    /// <summary>
    /// Calculate when the player has collected some amount of fruits and changes it for player lifes.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void FruitsForLifes()
    {
        if(fruits >= fruits4lifes)
        {
            fruits = 0;

            int currentLifes = int.Parse(lifesText.text);

            currentLifes++;

            SetPlayerLifes(currentLifes);
        }
    }

    private void CollectKey(int key, int keyScore)
    {
        StartCoroutine(AddKey(key, keyScore));
    }

    private IEnumerator AddKey(int key, int keyScore)
    {
        if(!isCollectingItem)
        {
            isCollectingItem = true;

            keysCollected += key;

            IncreseScore(keyScore);

            if(keysCollected.Equals(keysToCollect))
                levelComplete = true;
            
            yield return new WaitForSeconds(.15f);

            isCollectingItem = false;
        }
    }

    public void ShowGameOverMenu()
    {
        Time.timeScale = 0f;

        gameOverScreen.SetActive(true);

        isGameOver = true;
    }

    private void DisableEnemyHitPoint()
    {
        if(gameMode.Equals(GameMode.regular))
        {
            GameObject[] hitPoints = GameObject.FindGameObjectsWithTag("HitPoint");

            foreach(GameObject hitPoint in hitPoints)
                hitPoint.SetActive(false);
        }
    }

    public GameMode GetGameMode()
    {
        return this.gameMode;
    }
    public GameAction GetGameAction()
    {
        return this.action;
    }

    public Ladder GetLadder()
    {
        return this.ladder;
    }

    public void SetGameAction(GameAction action)
    {
        this.action = action;
    }

    public void SetLadder(Ladder ladder)
    {
        this.ladder = ladder;
    }

    public void SetPlayerLifes(int playerLifes)
    {
        this.playerLifes = playerLifes;
    }
}
