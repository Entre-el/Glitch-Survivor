using TMPro;
using UnityEngine;

public class BuffUIItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer iconRenderer;

    [SerializeField]
    private TextMeshPro stackText;

    public void Setup(Sprite icon, int stackCount)
    {
        iconRenderer.sprite = icon;

        if (stackCount > 1)
        {
            stackText.text = stackCount.ToString();
            stackText.enabled = true;
        }
        else
        {
            stackText.enabled = false;
        }
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
