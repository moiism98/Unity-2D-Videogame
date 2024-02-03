using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenZone : MonoBehaviour
{
    [HideInInspector] public HiddenZoneType hiddenZoneType = HiddenZoneType.tilemap;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Tilemap tilemap;
    private Color defaultColor;
    [SerializeField] private Color transparentColor;
    private float fadeTime = .5f;
    private bool fadeOut = true;
    private bool zoneDiscovered = false;
    private Coroutine fadeOutCoroutine;
    void Start()
    {
        SetDefaultColor();        
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {       
            if(!zoneDiscovered || hiddenZoneType.Equals(HiddenZoneType.gameObject)) // we only want this effect looping on gameObjects, not on tilemaps avoiding some visual bugs!
            {
                if(fadeOutCoroutine != null)
                    StopCoroutine(fadeOutCoroutine);

                fadeOutCoroutine = StartCoroutine(FadeZone(fadeOut));
            }    
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(hiddenZoneType.Equals(HiddenZoneType.gameObject)) // we only want this effect looping on gameObjects, not on tilemaps avoiding some visual bugs!
            {
                if(fadeOutCoroutine != null)
                    StopCoroutine(fadeOutCoroutine);

                fadeOutCoroutine = StartCoroutine(FadeZone(!fadeOut));
            }
        }
    }

    /// <summary>
    /// Triggers the hidden zone's fade effect when detects the player nearby.
    /// </summary>
    /// <param name="isEnteringOnTheZone"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="isEnteringOnTheZone"></param>
    /// <returns></returns>
    private IEnumerator FadeZone(bool isEnteringOnTheZone)
    {
        zoneDiscovered = true;

        Color startColor = defaultColor;

        Color targetColor = transparentColor;

        if(!isEnteringOnTheZone)
        {
            startColor = transparentColor;

            targetColor = defaultColor;
        }

        float timeFading = 0f;

        while(timeFading < fadeTime)
        {
            switch(hiddenZoneType)
            {
                case HiddenZoneType.tilemap: tilemap.color = Color.Lerp(startColor, targetColor, timeFading / fadeTime); break;
                case HiddenZoneType.gameObject: spriteRenderer.color = Color.Lerp(startColor, targetColor, timeFading / fadeTime); break;
            }

            timeFading += Time.deltaTime;

            yield return null;
        }
    }

    /// <summary>
    /// Sets the hidden zone's default color.
    /// </summary>
    private void SetDefaultColor()
    {
        if(hiddenZoneType.Equals(HiddenZoneType.tilemap))
            defaultColor = tilemap.color;
        else
            defaultColor = spriteRenderer.color;
    }
}
