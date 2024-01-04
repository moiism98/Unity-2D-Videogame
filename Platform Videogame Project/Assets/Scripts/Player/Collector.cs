using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Gem gem = collision.GetComponent<Gem>();

        gem?.Collect();

        ArrowItem arrow = collision.GetComponent<ArrowItem>();

        arrow?.Collect();

        Heart heart = collision.GetComponent<Heart>();

        heart?.Collect();
    }
}
