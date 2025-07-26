using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AdaptiveScrollView : MonoBehaviour
{
    public RectTransform content;

    private async void OnEnable() => await UpdateContentSize();
    
    public Task UpdateContentSize()
    {
        float totalHeight = 0f;

        foreach (RectTransform child in content)
            totalHeight += child.sizeDelta.y;

        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight / 1.65f);
        return Task.CompletedTask;
    }
}