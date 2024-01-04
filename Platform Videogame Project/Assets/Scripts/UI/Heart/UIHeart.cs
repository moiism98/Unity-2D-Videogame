using UnityEngine;
using UnityEngine.UI;

public class UIHeart : MonoBehaviour
{
    [SerializeField] private HeartType heartType = HeartType.heart;
    [SerializeField] private Sprite heart;
    [SerializeField] private Sprite emptyHeart;
    private Image heartImage;

    private void Start()
    {
        heartImage = GetComponent<Image>();
    }

    private void Update()
    {
        switch(heartType)
        {
            case HeartType.heart: heartImage.sprite = heart; break;
            case HeartType.emptyHeart: heartImage.sprite = emptyHeart; break;
        }
    }

    public HeartType GetHeartType()
    {
        return this.heartType;
    }

    public void SetHeartType(HeartType heartType)
    {
        this.heartType = heartType;
    }
}
