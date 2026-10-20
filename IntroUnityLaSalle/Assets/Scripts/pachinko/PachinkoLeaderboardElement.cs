using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PachinkoLeaderboardElement : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;
    public Color[] colors;

    public void SetText(string t)
    {
        text.text = t;
    }

    public void SetIndexData(int index)
    {
        image.rectTransform.anchoredPosition = new Vector2(image.rectTransform.anchoredPosition.x, image.rectTransform.sizeDelta.y * index);
        image.color = colors[index % colors.Length];
    }
}
