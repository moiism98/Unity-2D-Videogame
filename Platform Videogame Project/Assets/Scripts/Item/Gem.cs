using System;
using TMPro;
using UnityEngine;

public class Gem : MonoBehaviour, ItemInterface
{
    public static event Action<int> OnGemCollect;
    [SerializeField] private int gemValue = 1000;
    [SerializeField] private GameObject gemScore;
    public void Collect()
    {
        OnGemCollect.Invoke(gemValue);

        Destroy(gameObject);

        gemScore.GetComponent<TextMeshPro>().text = "+ " + gemValue.ToString();

        Instantiate(gemScore, transform.position, Quaternion.identity);
    }
}
