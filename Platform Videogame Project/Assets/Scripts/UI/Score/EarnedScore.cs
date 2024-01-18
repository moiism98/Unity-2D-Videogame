using TMPro;
using UnityEngine;

public class EarnedScore : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 1.5f);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + .01f), 1);
    }
}
