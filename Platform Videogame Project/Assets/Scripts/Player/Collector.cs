using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Gem gem = collider.GetComponent<Gem>();

        gem?.Collect();
    }
}
