using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIResizeUtility : MonoBehaviour
{
    #region Change BOTH Width and Height

    [MenuItem("L-Resizer/Percentage/Width and Height/10% of Parent WH")]
    private static void ResizeElement10ParentWH()
    {
        ResizeElementInternalEditor(0.1f, 0.1f, true);
    }

    [MenuItem("L-Resizer/Percentage/Width and Height/25% of Parent WH")]
    private static void ResizeElement25ParentWH()
    {
        ResizeElementInternalEditor(0.25f, 0.25f, true);
    }

    [MenuItem("L-Resizer/Percentage/Width and Height/100% of Parent WH")]
    private static void ResizeElementFullParentWH()
    {
        ResizeElementInternalEditor(1, 1, true);
    }

    [MenuItem("L-Resizer/Percentage/Width and Height/50% of Parent WH")]
    private static void ResizeElementHalfParentWH()
    {
        ResizeElementInternalEditor(0.5f, 0.5f, true);
    }

    #endregion Change BOTH Width and Height

    #region Change ONLY Width

    [MenuItem("L-Resizer/Percentage/Width/10% of Parent Width")]
    private static void ResizeElement10ParentWidth()
    {
        ResizeElementInternalEditor(0.1f, -1, true);
    }

    [MenuItem("L-Resizer/Percentage/Width/25% of Parent Width")]
    private static void ResizeElement25ParentWidth()
    {
        ResizeElementInternalEditor(0.25f, -1, true);
    }

    [MenuItem("L-Resizer/Percentage/Width/100% of Parent Width")]
    private static void ResizeElementFullParentWidth()
    {
        ResizeElementInternalEditor(1, -1, true);
    }

    [MenuItem("L-Resizer/Percentage/Width/50% of Parent Width")]
    private static void ResizeElementHalfParentWidth()
    {
        ResizeElementInternalEditor(0.5f, -1, true);
    }

    #endregion Change ONLY Width

    #region Change ONLY Height

    [MenuItem("L-Resizer/Percentage/Height/10% of Parent Height")]
    private static void ResizeElement10ParentHeight()
    {
        ResizeElementInternalEditor(-1, 0.1f, true);
    }

    [MenuItem("L-Resizer/Percentage/Height/25% of Parent Height")]
    private static void ResizeElement25ParentHeight()
    {
        ResizeElementInternalEditor(-1, 0.25f, true);
    }

    [MenuItem("L-Resizer/Percentage/Height/100% of Parent Height")]
    private static void ResizeElementFullParentHeight()
    {
        ResizeElementInternalEditor(-1, 1, true);
    }

    [MenuItem("L-Resizer/Percentage/Height/50% of Parent Height")]
    private static void ResizeElementHalfParentHeight()
    {
        ResizeElementInternalEditor(-1, 0.5f, true);
    }

    #endregion Change ONLY Height

    public static void ResizeElement(GameObject go, float width, float height, bool usePercentage)
    {
        if (go == null)
            return;

        RectTransform thisRT = GetRectTransformFromGameObject(go);
        if (thisRT != null)
        {
            RectTransform parentRT = thisRT.parent.GetComponent<RectTransform>();
            if (parentRT == null)
                return;

            ResizeElementByPXAndPercentage(thisRT, parentRT, usePercentage, width, height);
        }
    }

    private static Dictionary<GameObject, RectTransform> _cachedTransforms = new Dictionary<GameObject, RectTransform>();

    private static RectTransform GetRectTransformFromGameObject(GameObject go)
    {
        if (go == null)
            return null;

        if (_cachedTransforms.ContainsKey(go))
        {
            Debug.Log("Rect is Cached");
            return _cachedTransforms[go];
        }
        else
        {
            Debug.Log("Rect is not Cached");
            _cachedTransforms.Add(go, go.GetComponent<RectTransform>());
            return _cachedTransforms[go];
        }
    }

    private static void ResizeElementByPXAndPercentage(RectTransform thisRT, RectTransform parentRT, bool usePercentages, float desiredWidth = -1, float desiredHeight = -1)
    {
        // 1. Store element's anchor positions
        // 2. Set anchors to the element's center
        // 3. Resize Element using its' sizeDelta
        // 4. Reposition the element's anchors

        float rectFinalWidth = thisRT.rect.width;
        float rectFinalHeight = thisRT.rect.height;
        thisRT.anchorMax = new Vector2(0.5f, 0.5f);
        thisRT.anchorMin = new Vector2(0.5f, 0.5f);
        thisRT.position = parentRT.position;

        if (usePercentages)
        {
            if (desiredWidth > 0)
                rectFinalWidth = parentRT.rect.width * desiredWidth;
            if (desiredHeight > 0)
                rectFinalHeight = parentRT.rect.height * desiredHeight;
        }
        else
        {
            if (desiredWidth > 0)
                rectFinalWidth = desiredWidth;
            if (desiredHeight > 0)
                rectFinalHeight = desiredHeight;
        }

        thisRT.sizeDelta = new Vector2(rectFinalWidth, rectFinalHeight);
        Debug.Log(thisRT.sizeDelta);
    }

    private static void ResizeElementInternalEditor(float width, float height, bool usePercentage)
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            ResizeElement(go, width, height, usePercentage);
        }
    }
}