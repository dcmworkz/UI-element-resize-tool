using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lairinus.UI.Editor
{
    public static class UIResizedWindowUtilityHelper
    {
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
    }
}