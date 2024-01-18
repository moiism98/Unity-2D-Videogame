using UnityEngine;

public class Bush : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color defaultColor = new Color(1f, 1f, 1f, 1f);
    private Color transparentColor = new Color(1f, 1f, 1f, .5f);
    void Start()
    {
        spriteRenderer =  GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            spriteRenderer.color = transparentColor;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            spriteRenderer.color = defaultColor;
    }
}
