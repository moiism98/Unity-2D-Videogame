using UnityEngine;

public class EarnedScore : MonoBehaviour
{
    void Start()
    {
        // when the UI is spawned, will be destroyed after a certain amount of seconds.

        Destroy(gameObject, 1.5f);
    }

    void Update()
    {
        // since the UI is spawned, will be moving up until the "death time" arrives.

        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + .01f), 1);
    }
}
