using System;
using UnityEngine;

public class Fruit : MonoBehaviour, ItemInterface
{
    public static event Action<int> OnFruitCollect;
    [SerializeField] private int fruitScore = 200;
    [SerializeField] private GameObject collectAnimation;
    private GameController gameController;
    private AudioManager audioManager;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        audioManager = FindObjectOfType<AudioManager>();
    }

    public void Collect()
    {
        OnFruitCollect.Invoke(fruitScore);

        Instantiate(collectAnimation, transform.position, Quaternion.identity);

        gameController.ShowEarnedScore(fruitScore, transform);

        Destroy(gameObject);

        audioManager.PlaySound("Heart, fruit and arrow");
    }
}
