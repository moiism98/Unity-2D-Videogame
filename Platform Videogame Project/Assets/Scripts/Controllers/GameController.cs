using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    [Header("Static variables")]
    private int stageID = 1;
    private int totalScore = 0;
    private int sBonusScore;
    private int playerLifes;

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
        [SerializeField] private float transitionTime = 3.5f;
        [SerializeField] private TextMeshProUGUI transitionScore;
        [SerializeField] private TextMeshProUGUI transitionLifes;
        [HideInInspector] public TextMeshProUGUI currentStageText;
        [HideInInspector] public TextMeshProUGUI lastStageText;

        [Header("Pause UI")]
        [SerializeField] GameObject pauseScreen;
        [SerializeField] private Button pausedSelectedButton;
        public static bool isGamePaused = false;

        [Header("Game Over UI")]
        [HideInInspector] public GameObject gameOverScreen;
        [HideInInspector] public Button retryButton;
        [HideInInspector] public Button quitButton;
        public static bool isGameOver = false;


        [Header("Player Health")]
        [SerializeField] private GameObject healthContainer;
        [SerializeField] private GameObject heart;

        [Header("Player Lifes")]
        [SerializeField] private TextMeshProUGUI lifesText;
        [SerializeField] private int startingLifes = 3;
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
    private AudioManager audioManager;

    [Header("Climb variables")]
    private Ladder ladder;

    private void Awake()
    {
        LoadInitialStage();

        SetInitalUI();
    }
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        deviceController = FindObjectOfType<DeviceController>();
        
        audioManager = FindObjectOfType<AudioManager>();

        StartCoroutine(ResetTimeScale());

        playerLifes = startingLifes;

        SetPlayerHealth();

        // we suscribe the method to the game events

        GameEvents();
    }
    
    private void Update()
    {
        // if we are close to a ladder in the level, we can show the corresponding controller button (based in which controller we are using right now)

        ladder?.ShowControllerButton();

        arrow.SetActive(isArrowReady);

        FruitsForLifes();

        UpdateScore();
    }

    /// <summary>
    /// Loads the current level stage and also reloads the player and his spawn position.
    /// </summary>
    private void LoadStage()
    {
        GameObject level = GameObject.FindGameObjectWithTag("Level");

        if(level != null) // if we found a level we destroy it to load a new one!
        {
            Destroy(level);
        }

        GameObject stage = null;

        switch(gameMode)
        {
            case GameMode.regular: stage = SelectStage().GetStage(); break; // if regular mode, load the correspondant stage of the level

            case GameMode.bonus: 

                int randomBonus = UnityEngine.Random.Range(0, selectedLevel.GetBonus().Count); // if it is a bonus level we load a random bonus stage!
            
                stage = selectedLevel.GetBonus()[randomBonus]; 
                
            break;
        }

        if(stage != null) // if we've found the correspondant stage, we instantiate it!
            Instantiate(stage, transform.position, Quaternion.identity);
        
        DisableEnemyHitPoint(); // this is only for the regular game mode!

        SetPlayer();

        keysCollected = 0;

        levelComplete = false;
    }

    /// <summary>
    /// Selects a stage to load from the level's stage array. We select the correspondant one based on the stage ID.
    /// </summary>
    /// <returns></returns>
    private Stage SelectStage()
    {
        Stage[] levelStages = selectedLevel.GetStages();

        Stage stage = Array.Find(levelStages, stage => stage.GetIndex().Equals(stageID));

        if(stage != null)
            currentStageText.text = stage.GetIndex().ToString();

        currentStage = stage;

        return stage;
    }

    /// <summary>
    /// This method move the player to the correspondant stage's spawn point or spawn the player only when the game starts!
    /// </summary> <summary>
    /// 
    /// </summary>
    private void SetPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            // we delay the method so the stage can spawn and then move the player to the spawn point (stage's start position)

            // if we don't delay it, the spawn position still being the last one, we have to wait for the game to instantiate the new stage first, then take it's spawn point!

            Invoke("MovePlayer", .01f);
        }
        else
            SpawnPlayer();
    }

    /// <summary>
    /// Spawns the player when the game starts.
    /// </summary>
    private void SpawnPlayer()
    {
        Transform spawnPoint = GameObject.FindGameObjectWithTag("Spawn Point").transform;

        Instantiate(selectedLevel.GetPlayer(), spawnPoint.position, Quaternion.identity);
    }

    /// <summary>
    /// Move the player on the next stage's start position.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void MovePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Transform spawnPoint = GameObject.FindGameObjectWithTag("Spawn Point").transform;

        if(player != null)
            player.transform.position = spawnPoint.position;
            
    }

    private void SetInitalUI()
    {
        gameUIPanel.SetActive(false);

        transitionScreen.SetActive(true);

        bonusTransitionScreen.SetActive(false);

        gameOverScreen.SetActive(false);

        pauseScreen.SetActive(isGamePaused);

        // set this 2 variables there because we want to reset to default values when the scene is reloaded. 
        // p.e if you die with an arrow in your pocket and the scene is reloaded (you ran out of lifes), you start the game again with the arrow activated and you can use it!

        isArrowReady = false;

        fruits = 0;
    }

    /// <summary>
    /// Load the first stage of the first array's level. This only happens at the game's start.
    /// </summary>
    private void LoadInitialStage()
    {
        selectedLevel = Array.Find(levels, level => level.GetName().Equals(levelName));

        if(selectedLevel != null)
        {
            lastStageText.text = selectedLevel.GetStages().Length.ToString();

            LoadStage();
        }
    }

    private void GameEvents()
    {
        Key.OnKeyCollect += CollectKey;

        Gem.OnGemCollect += IncreseScore;

        Heart.OnHeartCollect += HealPlayer;

        Fruit.OnFruitCollect += CollectFruit;

        EnemyController.OnEnemyDie += IncreseScore;
    }

    /// <summary>
    /// Loads the next stage of the current level.
    /// </summary> <summary>
    /// 
    /// </summary>
    public void LoadNextStage()
    {
        switch(gameMode)
        {
            case GameMode.regular:

                if(levelComplete && Exit.nearExit)
                {
                    //gameMode = GameMode.bonus;

                    if(stageID.Equals(selectedLevel.GetStages().Length))
                    {
                        // load main menu scene, we end the game!

                        SceneManager.LoadScene("MainScene");
                    }

                    audioManager.StopSound(GetStageAudioClip());

                    stageID++;

                    LoadStage();

                    Time.timeScale = 0.0f;

                    PlayTransition(); // code this method, tho show the transition screen again!

                    StartCoroutine(ResetTimeScale());
                }

            break;
        }
        
    }

    public void Retry()
    {
        if(playerLifes >= 0)
            ReloadLevel();
        else
            ReloadScene();
    }

    /// <summary>
    /// Reloads the current level where the player is playing and dies in, resets the current levels score earned and put everything back in place again.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void ReloadLevel()
    { 
        isGameOver = !isGameOver;

        audioManager.StopSound("Game Over");

        SetInitalUI();

        arrow.SetActive(false);

        LoadStage();

        playerController.SetHealth(playerController.GetMaxHealth());

        SetPlayerHealth();

        StartCoroutine(ResetTimeScale());
    }

    /// <summary>
    /// Reloads the game scene, its happens when the player ran out of lives and he has to start the game again.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Resets the time scale using the unscaled time, this allows us to show the transition screens while the game is paused.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetTimeScale()
    {
        yield return new WaitForSecondsRealtime(transitionTime);

        Time.timeScale = 1.0f;

        StopTransition();
    }

    private void PlayTransition()
    {
        if(gameMode.Equals(GameMode.regular))
            transitionScreen.SetActive(true);
        else
            bonusTransitionScreen.SetActive(true);
    }

    private void StopTransition()
    { 
        switch(gameMode)
        {
            case GameMode.regular: 

                transitionScreen.SetActive(false);

                audioManager.PlaySound(GetStageAudioClip());

                playerController.SetIsHurt(false);

            break;

            case GameMode.bonus: 

                // transitionLoaded = true;
            
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

            Time.timeScale = 0.0f;

            deviceController.UpdateGameActions("Menu");
        }
        else 
        {
            pauseScreen.SetActive(false);

            Time.timeScale = 1.0f;

            deviceController.UpdateGameActions("Player");
        }
    }

    /// <summary>
    /// Shows the 'Game Over' screen.
    /// </summary>
    public void ShowGameOverMenu()
    {
        Time.timeScale = 0.0f;

        gameOverScreen.SetActive(true);

        audioManager.StopSound(GetStageAudioClip());

        audioManager.PlaySound("Game Over");

        isGameOver = true;

        playerLifes--;
    }

    /// <summary>
    /// Sets everything related with the score (keys, player lifes, score...)
    /// </summary> <summary>
    /// 
    /// </summary>
    private void UpdateScore()
    {
        keysText.text = keysCollected.ToString();

        scoreText.text = totalScore.ToString();

        if(playerLifes >= 0)
            lifesText.text = playerLifes.ToString();
        else lifesText.text = "0";

        transitionLifes.text = playerLifes.ToString();

        transitionScore.text = "Score: " + scoreText.text;
    }

    /// <summary>
    /// Triggers the coroutine that increase or set the game score.
    /// </summary>
    /// <param name="gemValue"></param>
    private void IncreseScore(int score)
    {
        if(this != null)
            StartCoroutine(SetNewScore(score)); 
    }
    
    /// <summary>
    /// Set the game score based on score points earned.
    /// </summary>
    /// <param name="gemValue"></param>
    /// <returns></returns>
    private IEnumerator SetNewScore(int score)
    {
        totalScore = int.Parse(scoreText.text) + score;

        yield return new WaitForSeconds(.1f);
    }
    
    /// <summary>
    /// Display the points player earn.
    /// </summary>
    /// <param name="score"></param>
    /// <param name="scorePosition"></param> <summary>
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
        Transform[] previousHearts = healthContainer.GetComponentsInChildren<Transform>();

        if(previousHearts != null && previousHearts.Length > 0)
        {
            foreach(Transform heart in previousHearts)
            {
                if(heart.name.Contains("Heart"))
                    Destroy(heart.gameObject);
            }
        }

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
        if(this != null)
            StartCoroutine(HealCoroutine(health));
    }

    /// <summary>
    /// Waits an amount of seconds for player's heal preventing him healing twice.
    /// </summary>
    /// <param name="health"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="health"></param>
    /// <returns></returns>
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
        if(this != null)
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

            playerLifes = currentLifes;
        }
    }

    private void CollectKey(int key, int keyScore)
    {
        if(this != null)
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

    public string GetStageAudioClip()
    {
        return currentStage.GetStage().name;
    }

    public void SetGameAction(GameAction action)
    {
        this.action = action;
    }

    public void SetLadder(Ladder ladder)
    {
        this.ladder = ladder;
    }

    public void SetPlayerLifes(int startingLifes)
    {
        this.startingLifes = startingLifes;
    }
}
