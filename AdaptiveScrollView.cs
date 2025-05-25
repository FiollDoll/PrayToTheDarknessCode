using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AdaptiveScrollView : MonoBehaviour
{
    public RectTransform content;

    private void OnEnable() => UpdateContentSize();
    
    public void UpdateContentSize()
    {
        float totalHeight = 0f;

        foreach (RectTransform child in content)
            totalHeight += child.sizeDelta.y;

        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight / 1.65f);
    }
}