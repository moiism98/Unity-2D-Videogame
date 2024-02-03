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

        Fruit fruit = collision.GetComponent<Fruit>();

        fruit?.Collect();

        Key key = collision.GetComponent<Key>();

        key?.Collect();
    }
}
