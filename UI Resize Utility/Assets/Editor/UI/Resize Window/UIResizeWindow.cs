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

        private bool _adjustHeight = true;

        private bool _adjustWidth = true;

        private bool _changeSizeBasedOnParent = true;

        private float _desiredHeight = 0;

        private float _desiredWidth = 0;

        private bool _keepElementsOldPosition = true;

        private ResizeUnitType _resizeType = ResizeUnitType.Percentage;

        [MenuItem("Window/Lairinus/Resizer")]
        private static void ShowWindow()
        {
            /*
             * Internal use
             * -------------------------
             * Instantiates an instance of this class
             */

            EditorWindow ew = GetWindow(typeof(UIResizeWindow));
            ew.minSize = new Vector2(500, 500);
        }

        private static void UpdateSelectedObject()
        {
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
            ShowGUI_Overview();
            ShowGUI_ElementConfiguration();
            ShowGUI_ResizeConfiguration();
            ShowGUI_FinalizeResize();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void ResizeElementByPXAndPercentage(RectTransform thisRT, RectTransform parentRT, bool usePercentages, float desiredWidth = -1, float desiredHeight = -1)
        {
            /*
             * Internal use only
             * -----------------
             * Processes resizing the elements with the provided parameters inside of this class
             */

            Vector2 oldPos = thisRT.position;
            float rectFinalWidth = thisRT.rect.width;
            float rectFinalHeight = thisRT.rect.height;
            thisRT.anchorMax = new Vector2(0.5f, 0.5f);
            thisRT.anchorMin = new Vector2(0.5f, 0.5f);

            if (!_changeSizeBasedOnParent)
                parentRT = thisRT;

            if (!_keepElementsOldPosition)
                thisRT.position = parentRT.position;
            else
                thisRT.position = oldPos;

            if (usePercentages)
            {
                if (parentRT != null)
                {
                    if (desiredWidth > 0 && _adjustWidth)
                        rectFinalWidth = parentRT.rect.width * (desiredWidth / 100);
                    if (desiredHeight > 0 && _adjustHeight)
                        rectFinalHeight = parentRT.rect.height * (desiredHeight / 100);
                }
            }
            else
            {
                if (desiredWidth > 0 && _adjustWidth)
                    rectFinalWidth = desiredWidth;
                if (desiredHeight > 0 && _adjustHeight)
                    rectFinalHeight = desiredHeight;
            }

            thisRT.sizeDelta = new Vector2(rectFinalWidth, rectFinalHeight);
        }

        private void ShowGUI_ElementConfiguration()
        {
            EditorGUILayout.LabelField("Element Configuration:", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            _keepElementsOldPosition = EditorGUILayout.Toggle(new GUIContent("Keep Element's Position", "If the position is not kept, the element will be centered."), _keepElementsOldPosition);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            _adjustWidth = EditorGUILayout.Toggle(new GUIContent("AdjustWidth", "Shows controls for modifying the element's width"), _adjustWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            _adjustHeight = EditorGUILayout.Toggle("Adjust Height", _adjustHeight);
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Space(30);
        }

        private void ShowGUI_FinalizeResize()
        {
            if (GUILayout.Button("Resize Element"))
            {
                foreach (RectTransform rt in _selectedRectTransforms)
                {
                    RectTransform parentRT = null;
                    if (rt.parent != null)
                        parentRT = rt.parent.GetComponent<RectTransform>();

                    ResizeElementByPXAndPercentage(rt, parentRT, _resizeType == ResizeUnitType.Percentage, _desiredWidth, _desiredHeight);
                }
            }
        }

        private void ShowGUI_Overview()
        {
            /*
             * Internal use only
             * -----------------
             * Shows general information about the Selected GameObjects
             */

            EditorGUILayout.LabelField("Overview", EditorStyles.boldLabel);
            if (Selection.gameObjects.Length != _selectedRectTransforms.Count)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                EditorGUILayout.HelpBox("Some of the selected GameObjects do not have a RectTransform. Only GameObjects with a RectTransform can have their size adjusted", MessageType.Warning);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            EditorGUILayout.LabelField(_selectedRectTransforms.Count + " Resizable element(s) are selected!");
            GUILayout.EndHorizontal();

            GUILayout.Space(30);
        }

        private void ShowGUI_ResizeConfiguration()
        {
            GUILayout.Label("Size Configuration", EditorStyles.boldLabel);

            // Use Pixels
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUIContent percentContent = new GUIContent("Use Percentage", "Percentage bases its' size off of the Parent Rect (if selected) or off of the element itself. If an element is 100 pixels wide, and you wish to double it, use 200%");
            GUIContent pixelsContent = new GUIContent("Use Pixels", "Using pixels resizes an element to the exact Pixel specifications that you want.");
            _resizeType = (ResizeUnitType)GUILayout.SelectionGrid((int)_resizeType, new GUIContent[] { percentContent, pixelsContent }, 2);
            GUILayout.EndHorizontal();

            // Determine if we want to base the Percentage change based off the parent rect
            if (_resizeType == ResizeUnitType.Percentage)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUIContent baseOffParentRect = new GUIContent("Base off Parent Rect", "Values based off of the Parent Rect will be a percentage of that parent. For instance, if an Image is 100x100 and you want it to be 50% the width of its' parent (500x500), the final size will be 250x250");
                _changeSizeBasedOnParent = EditorGUILayout.Toggle(baseOffParentRect, _changeSizeBasedOnParent);
                GUILayout.EndHorizontal();
            }

            // Percent Width
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if (_resizeType == ResizeUnitType.Percentage)
            {
                if (_adjustWidth)
                    _desiredWidth = EditorGUILayout.Slider("Width (in Percentage)", _desiredWidth, 0.5f, 500);
            }
            GUILayout.EndHorizontal();

            // Percent Height
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if (_resizeType == ResizeUnitType.Percentage)
            {
                if (_adjustHeight)
                    _desiredHeight = EditorGUILayout.Slider("Height (in Percentage)", _desiredHeight, 0.5f, 500);
            }
            GUILayout.EndHorizontal();

            // Pixel Width
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if (_resizeType == ResizeUnitType.Pixels)
            {
                if (_adjustWidth)
                    _desiredWidth = EditorGUILayout.Slider("Width (in Pixels)", _desiredWidth, 0.5f, 4000);
            }
            GUILayout.EndHorizontal();

            // Pixel Height
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if (_resizeType == ResizeUnitType.Pixels)
            {
                if (_adjustHeight)
                    _desiredHeight = EditorGUILayout.Slider("Height (in Pixels)", _desiredHeight, 0.5f, 4000);
            }
            GUILayout.EndHorizontal();
        }
    }
}