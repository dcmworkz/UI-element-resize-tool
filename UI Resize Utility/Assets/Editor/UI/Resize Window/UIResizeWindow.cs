using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Lairinus.UI.Editor
{
    public class UIResizeWindow : EditorWindow
    {
        public enum ResizeUnitType
        {
            Percentage,
            Pixels
        }

        private static List<RectTransform> _selectedRectTransforms = new List<RectTransform>();

        private bool _alterSizeBasedOnParent = true;

        private float _desiredHeight = 0;

        private float _desiredWidth = 0;

        private bool _keepPosition = true;

        private bool _hideWarningMessages = false;

        private bool _enableDebugging = false;

        private ResizeUnitType _resizeType = ResizeUnitType.Percentage;

        [MenuItem("Window/Lairinus/UI Element Resizer")]
        private static void ShowWindow()
        {
            /*
             * Internal use
             * -------------------------
             * Instantiates an instance of this class.
             * Only one instance of this class can be active at a time
             */

            EditorWindow ew = GetWindow(typeof(UIResizeWindow), false, "UI Resizer");
            ew.minSize = new Vector2(500, 500);
        }

        private void UpdateSelectedObject()
        {
            /*
             * Internal use
             * -------------------------
             * Each time an object is selected in the Scene, it attempts to update
             * the currently selected Rect Transforms
             */

            _selectedRectTransforms.Clear();
            foreach (GameObject go in Selection.gameObjects)
            {
                RectTransform rt = go.GetComponent<RectTransform>();
                if (rt != null)
                    _selectedRectTransforms.Add(rt);
            }
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= UpdateSelectedObject;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += UpdateSelectedObject;
        }

        private void OnGUI()
        {
            try
            {
                ShowGUI_WindowConfiguration();
                ShowGUI_ResizeConfiguration();
                ShowGUI_ResizeConfirmation();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Uncaught Exception at " + ex + " please contact lairinus@gmail.com for information on how to get this resolved!");
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void ResizeElementInternal(RectTransform thisRT, RectTransform parentRT, float desiredWidth = -1, float desiredHeight = -1)
        {
            /*
             * Internal use only
             * -----------------
             * Processes resizing the elements with the provided parameters inside of this class
             */

            if (parentRT == null)
            {
                parentRT = thisRT;
                if (_resizeType == ResizeUnitType.Percentage && _alterSizeBasedOnParent)
                    Debug.LogWarning(string.Format(Debugger.parentRectIsNull, thisRT.name));
            }

            Vector2 oldPos = thisRT.position;
            float rectFinalWidth = thisRT.rect.width;
            float rectFinalHeight = thisRT.rect.height;
            thisRT.anchorMax = new Vector2(0.5f, 0.5f);
            thisRT.anchorMin = new Vector2(0.5f, 0.5f);

            if (!_keepPosition)
                thisRT.position = parentRT.position;
            else
                thisRT.position = oldPos;

            if (!_alterSizeBasedOnParent)
            {
                parentRT = thisRT;
            }

            if (_resizeType == ResizeUnitType.Percentage)
            {
                if (parentRT != null)
                {
                    if (desiredWidth > 0)
                        rectFinalWidth = parentRT.rect.width * (desiredWidth / 100);
                    if (desiredHeight > 0)
                        rectFinalHeight = parentRT.rect.height * (desiredHeight / 100);
                }
            }
            else
            {
                if (desiredWidth > 0)
                    rectFinalWidth = desiredWidth;
                if (desiredHeight > 0)
                    rectFinalHeight = desiredHeight;
            }

            thisRT.sizeDelta = new Vector2(rectFinalWidth, rectFinalHeight);

            if (_enableDebugging)
                Debug.Log(string.Format(Debugger.finalElementSize, thisRT.name, rectFinalWidth, rectFinalHeight));
        }

        private void ShowGUI_WindowConfiguration()
        {
            /*
             * Internal use only
             * -----------------
             * Shows general information about the Selected GameObjects
             */

            // Header Box
            EditorGUILayout.HelpBox("Lairinus.UI UI Resizer Tool\n\nIn order to change the size of elements:\n1.Select UI Elements from the heirarchy\n2.Configure the desired sizes for the UI Elements\n3.Click the \"Resize\" button", MessageType.Info);

            _enableDebugging = EditorGUILayout.Toggle(new GUIContent("Enable Debugging", "Enabling debugging can show you exactly where and why something isn't being resized correctly."), _enableDebugging);
            _hideWarningMessages = EditorGUILayout.Toggle(new GUIContent("Hide Warning Messages", "Hiding Warning messages will clean up this Window's UI, however it is not recommended for new users"), _hideWarningMessages);
            GUILayout.Space(30);

            EditorGUILayout.LabelField(_selectedRectTransforms.Count + " Resizable element(s) are selected!", EditorStyles.boldLabel);

            if (Selection.gameObjects.Length != _selectedRectTransforms.Count)
            {
                if (!_hideWarningMessages)
                    EditorGUILayout.HelpBox("Some of the selected GameObjects do not have a RectTransform. Only GameObjects with a RectTransform can have their size adjusted", MessageType.Warning);
            }
            GUILayout.Space(30);
        }

        private void ShowGUI_ResizeConfiguration()
        {
            /*
             * Internal use only
             * -----------------
             * Shows the configuration options for the Elements to be resized
             */

            GUILayout.Label("Resize Configuration", EditorStyles.boldLabel);

            // Use Pixels or Percentages
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUIContent percentContent = new GUIContent("Use Percentage", "Percentage bases its' size off of the Parent Rect (if selected) or off of the element itself. If an element is 100 pixels wide, and you wish to double it, use 200%");
            GUIContent pixelsContent = new GUIContent("Use Pixels", "Using pixels resizes an element to the exact Pixel specifications that you want.");
            _resizeType = (ResizeUnitType)GUILayout.SelectionGrid((int)_resizeType, new GUIContent[] { percentContent, pixelsContent }, 2);
            GUILayout.EndHorizontal();

            // Keep Element Position
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            _keepPosition = EditorGUILayout.Toggle(new GUIContent("Keep Position", "If the position is not kept, the element will be centered. If the position is kept, the Element is resized, and then put back in its' original position afterward"), _keepPosition);
            GUILayout.EndHorizontal();

            // Determine if we want to base the Percentage change based off the parent rect
            if (_resizeType == ResizeUnitType.Percentage)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUIContent baseOffParentRect = new GUIContent("Use % of Parent Rect", "Values based off of the Parent Rect will be a percentage of that parent. For instance, if an Image is 100x100 and you want it to be 50% the width of its' parent (500x500), the final size will be 250x250. If this option is not flagged, the Image's final size will be 50x50");
                _alterSizeBasedOnParent = EditorGUILayout.Toggle(baseOffParentRect, _alterSizeBasedOnParent);
                GUILayout.EndHorizontal();
            }

            // Percent Width and Height
            if (_resizeType == ResizeUnitType.Percentage)
            {
                // Percent Width
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                _desiredWidth = EditorGUILayout.Slider("Width (in Percentage)", _desiredWidth, 0, 500);
                GUILayout.EndHorizontal();

                // Percent Height
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                _desiredHeight = EditorGUILayout.Slider("Height (in Percentage)", _desiredHeight, 0, 500);
                GUILayout.EndHorizontal();
            }

            // Pixel Width
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if (_resizeType == ResizeUnitType.Pixels)
            {
                // Pixel Width
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                _desiredWidth = EditorGUILayout.Slider("Width (in Pixels)", _desiredWidth, 0, 4000);
                GUILayout.EndHorizontal();

                // Pixel Height
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                _desiredHeight = EditorGUILayout.Slider("Height (in Pixels)", _desiredHeight, 0, 4000);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();

            // Warning - Width is <= 0
            if (_desiredWidth <= 0 && !_hideWarningMessages)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                EditorGUILayout.HelpBox("Warning - the Width is set to 0, so this Element's Width value will not be changed. In order to change the Width, set the Width to a value greater than 0.", MessageType.Warning);
                GUILayout.EndHorizontal();
            }

            // WArning - Height is <= 0
            if (_desiredHeight <= 0 && !_hideWarningMessages)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                EditorGUILayout.HelpBox("Warning - the Height is set to 0, so this Element's Height value will not be changed. In order to change the Height, set the Height to a value greater than 0.", MessageType.Warning);
                GUILayout.EndHorizontal();
            }
        }

        private void ShowGUI_ResizeConfirmation()
        {
            // Resize Element Button
            GUILayout.BeginArea(new Rect((Screen.width / 2) - 50, 440, 100, 100));
            if (GUILayout.Button(new GUIContent("Resize", "Applies the \"Resize Element\" changes that you've made."), GUILayout.Height(50)))
            {
                if (_selectedRectTransforms.Count == 0)
                {
                    if (_enableDebugging)
                    {
                        Debug.Log(Debugger.selectedRectTransform);
                        return;
                    }
                }

                foreach (RectTransform rt in _selectedRectTransforms)
                {
                    RectTransform parentRT = null;
                    if (rt.parent != null)
                        parentRT = rt.parent.GetComponent<RectTransform>();

                    ResizeElementInternal(rt, parentRT, _desiredWidth, _desiredHeight);
                }
            }
            GUILayout.EndArea();
        }

        private class Debugger
        {
            public const string finalElementSize = "UI Resizer DEBUGGING: The final size of {0} is {1} Width and {2} Height (in Pixels)";
            public const string selectedRectTransform = "UI Resizer DEBUGGING: There are no valid selected RectTransforms.\n In order to get valid Rect Transforms, select GameObjects in the heirarchy that have a RectTransform component";
            public const string parentRectIsNull = "UI Resizer DEBUGGING: GameObject {0} does not have a parent.\n No parent was found, but you specifed that you want to base this object's size off of the parent.";
        }
    }
}