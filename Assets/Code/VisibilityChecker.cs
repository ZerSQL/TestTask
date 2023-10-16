using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VisibilityChecker
{
    private static Vector3[] objectCorners = new Vector3[4];


    private static int GetVisibleCornersCount(this RectTransform rect, Camera camera)
    {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
        rect.GetWorldCorners(objectCorners);

        int cornersCount = 0;
        Vector3 tempCorner;

        for (var i = 0; i < objectCorners.Length; i++)
        {
            tempCorner = camera.WorldToScreenPoint(objectCorners[i]);
            if (screenBounds.Contains(tempCorner))
            {
                cornersCount++;
            }
        }
        return cornersCount;
    }

    public static bool IsRectVisible(this RectTransform rect, Camera camera)
    {
        return GetVisibleCornersCount(rect, camera) == 4;
    }
}
