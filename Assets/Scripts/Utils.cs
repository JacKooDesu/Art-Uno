using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float CanvasScaler
    {
        get => Screen.width / 1920f;
    }

    public static bool CheckRecttransformOverlaps(RectTransform r1, RectTransform r2)
    {
        UnityEngine.Debug.Log(r1.position);
        Rect rect1 = new Rect((Vector2)r1.position, r1.rect.size * CanvasScaler);
        Rect rect2 = new Rect((Vector2)r2.position, r2.rect.size * CanvasScaler);

        return rect1.Overlaps(rect2);
    }

    public static Vector2 GetRandomPointInRect(RectTransform rect, float offsetX = 0, float offsetY = 0)
    {
        Rect r = rect.rect;
        Vector2 v2 = new Vector2(
            Random.Range(-r.width / 2 + offsetX, r.width / 2 - offsetX),
            Random.Range(-r.height / 2 + +offsetY, r.height / 2 - offsetY));

        v2 *= CanvasScaler;

        v2 += (Vector2)rect.position;

        // UnityEngine.Debug.Log(rect.localPosition);
        // UnityEngine.Debug.Log(CanvasScaler);
        // UnityEngine.Debug.Log(Screen.resolutions[0].height);

        return v2;
    }
}
