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
    private Coroutine fadeOutCoroutine;
    void Start()
    {
        if(hiddenZoneType.Equals(HiddenZoneType.tilemap))
            defaultColor = tilemap.color;
        else
            defaultColor = spriteRenderer.color;        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {            
            if(fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);

            fadeOutCoroutine = StartCoroutine(FadeZone(fadeOut));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);

            fadeOutCoroutine = StartCoroutine(FadeZone(!fadeOut));
        }
    }

    private IEnumerator FadeZone(bool isEnteringOnTheZone)
    {
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

    private void FadeColor(Color startColor, Color targetColor)
    {
        float timeFading = 0f;

        while(timeFading < fadeTime)
        {
            switch(hiddenZoneType)
            {
                case HiddenZoneType.tilemap: tilemap.color = Color.Lerp(startColor, targetColor, timeFading / fadeTime); break;
                case HiddenZoneType.gameObject: spriteRenderer.color = Color.Lerp(startColor, targetColor, timeFading / fadeTime); break;
            }

            timeFading += Time.deltaTime;
        }
    }
}
