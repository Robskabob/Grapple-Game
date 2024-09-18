using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class UITest : UIBehaviour 
{
    public Vector2 anchoredPosition;
    public Vector2 offsetMax;
    public Vector2 offsetMin;
    public Vector2 anchorMax;
    public Vector2 anchorMin;
    public Vector2 sizeDelta;
    public bool Set;
	protected override void OnEnable()
	{
        RectTransform rect = transform as RectTransform;
        anchoredPosition = rect.anchoredPosition;
        offsetMax = rect.offsetMax;
        offsetMin = rect.offsetMin;
        anchorMax = rect.anchorMax;
        anchorMin = rect.anchorMin;
        sizeDelta = rect.sizeDelta;
    }

	private void Update()
	{
        RectTransform rect = transform as RectTransform;
        if (Set)
        {
            rect.offsetMax = offsetMax;
            rect.offsetMin = offsetMin;
            rect.anchorMax = anchorMax;
            rect.anchorMin = anchorMin;
            rect.sizeDelta = sizeDelta;
            rect.anchoredPosition = anchoredPosition;
        }
        anchoredPosition = rect.anchoredPosition;
        offsetMax = rect.offsetMax;
        offsetMin = rect.offsetMin;
        anchorMax = rect.anchorMax;
        anchorMin = rect.anchorMin;
        sizeDelta = rect.sizeDelta;
    }
}
