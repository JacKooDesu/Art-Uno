using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // public static float CanvasScaler{
    //     get{
    //         return Screen.resolutions[0].height
    //     }
    // }

    public static bool CheckRecttransformOverlaps(RectTransform r1, RectTransform r2)
    {
        Rect rect1 = new Rect((Vector2)r1.position, r1.rect.size);
        Rect rect2 = new Rect((Vector2)r2.position, r2.rect.size);

        return rect1.Overlaps(rect2);
    }

    public static Vector2 GetRandomPointInRect(RectTransform rect, float offsetX = 0, float offsetY = 0)
    {
        Rect r = rect.rect;
        Vector2 v2 = new Vector2(
            Random.Range(0 + offsetX, r.width - offsetX),
            Random.Range(0 + offsetY, r.height - offsetY));

        v2 += (Vector2)rect.position;

        UnityEngine.Debug.Log(rect.localPosition);
        // UnityEngine.Debug.Log(Screen.resolutions[0].height);

        return v2;
    }

    static void CanvasScaleConvert()
    {
        
    }
}
