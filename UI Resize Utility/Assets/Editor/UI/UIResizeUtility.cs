using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIResizeUtility : MonoBehaviour
{
    [MenuItem("Lairinus UI/Anchor Around Object")]
    private static void uGUIAnchorAroundObject()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            var o = go;
            if (o != null && o.GetComponent<RectTransform>() != null)
            {
                var r = o.GetComponent<RectTransform>();
                var p = o.transform.parent.GetComponent<RectTransform>();

                var offsetMin = r.offsetMin;
                var offsetMax = r.offsetMax;
                var _anchorMin = r.anchorMin;
                var _anchorMax = r.anchorMax;

                var parent_width = p.rect.width;
                var parent_height = p.rect.height;

                var anchorMin = new Vector2(_anchorMin.x + (offsetMin.x / parent_width),
                                            _anchorMin.y + (offsetMin.y / parent_height));
                var anchorMax = new Vector2(_anchorMax.x + (offsetMax.x / parent_width),
                                            _anchorMax.y + (offsetMax.y / parent_height));

                r.anchorMin = anchorMin;
                r.anchorMax = anchorMax;

                r.offsetMin = new Vector2(0, 0);
                r.offsetMax = new Vector2(0, 0);
                r.pivot = new Vector2(0.5f, 0.5f);
            }
        }
    }

    // --- Both --- //

    [MenuItem("L-Resizer/Width and Height/10% of Parent WH")]
    private static void ResizeElement10ParentWH()
    {
        ResizeElementInternalEditor(0.1f, 0.1f, true);
    }

    [MenuItem("L-Resizer/Width and Height/25% of Parent WH")]
    private static void ResizeElement25ParentWH()
    {
        ResizeElementInternalEditor(0.25f, 0.25f, true);
    }

    [MenuItem("L-Resizer/Width and Height/50% of Parent WH")]
    private static void ResizeElementHalfParentWH()
    {
        ResizeElementInternalEditor(0.5f, 0.5f, true);
    }

    [MenuItem("L-Resizer/Width and Height/100% of Parent WH")]
    private static void ResizeElementFullParentWH()
    {
        ResizeElementInternalEditor(1, 1, true);
    }

    // --- Width --- //

    [MenuItem("L-Resizer/Width/10% of Parent Width")]
    private static void ResizeElement10ParentWidth()
    {
        ResizeElementInternalEditor(0.1f, -1, true);
    }

    [MenuItem("L-Resizer/Width/25% of Parent Width")]
    private static void ResizeElement25ParentWidth()
    {
        ResizeElementInternalEditor(0.25f, -1, true);
    }

    [MenuItem("L-Resizer/Width/50% of Parent Width")]
    private static void ResizeElementHalfParentWidth()
    {
        ResizeElementInternalEditor(0.5f, -1, true);
    }

    [MenuItem("L-Resizer/Width/100% of Parent Width")]
    private static void ResizeElementFullParentWidth()
    {
        ResizeElementInternalEditor(1, -1, true);
    }

    // --- Height --- //

    [MenuItem("L-Resizer/Height/10% of Parent Height")]
    private static void ResizeElement10ParentHeight()
    {
        ResizeElementInternalEditor(-1, 0.1f, true);
    }

    [MenuItem("L-Resizer/Height/25% of Parent Height")]
    private static void ResizeElement25ParentHeight()
    {
        ResizeElementInternalEditor(-1, 0.25f, true);
    }

    [MenuItem("L-Resizer/Height/50% of Parent Height")]
    private static void ResizeElementHalfParentHeight()
    {
        ResizeElementInternalEditor(-1, 0.5f, true);
    }

    [MenuItem("L-Resizer/Height/100% of Parent Height")]
    private static void ResizeElementFullParentHeight()
    {
        ResizeElementInternalEditor(-1, 1, true);
    }

    private static void ResizeElementInternalEditor(float width, float height, bool usePercentage)
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            ResizeElement(go, width, height, usePercentage);
        }
    }

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

    private static Dictionary<GameObject, RectTransform> _cachedTransforms = new Dictionary<GameObject, RectTransform>();
}