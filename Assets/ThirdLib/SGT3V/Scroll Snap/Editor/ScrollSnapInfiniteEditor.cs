/* 
    Copyright (C) 2021 SGT3V, Sercan Altundas
    
    Visit for details: https://app.gitbook.com/@sercan-altundas/s/asset-store
*/

using UnityEditor;
using UnityEngine;
using SGT3V.Common.UI;

namespace SGT3V.ScrollSnap
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScrollSnapInfinite))]
    public sealed class ScrollSnapInfiniteEditor : Editor
    {
        private const string ScrollSnapInfiniteFileName = "Scroll Snap Infinite";

        private readonly GUIContent ContentLabel = new GUIContent("Content", "Container for snappable pages.");
        private readonly GUIContent SnapSpeedLabel = new GUIContent("Snap Speed", "Speed of snap, between 1 to 10. Value 10 snaps immediately.");
        private readonly GUIContent SnapAreaSizeLabel = new GUIContent("Snap Area Size", "Size of the center area that is necessary for activating the snap.");
        private readonly GUIContent ScrollSnapAxisLabel = new GUIContent("Scroll Snap Axis", "Axis of the ScrollSnap pages to align.");
        private readonly GUIContent VerticalPageStartPositionLabel = new GUIContent("Vertical Alignment", "Align start of the pages to the top or to the bottom of the Content.");
        private readonly GUIContent HorizontalPageStartPositionLabel = new GUIContent("Horizontal Alignment", "Align start of the pages to the left side or to the right side of the Content.");

        private SerializedProperty Content;
        private SerializedProperty SnapSpeed;
        private SerializedProperty SnapAreaSize;
        private SerializedProperty OnPageChanged;
        private SerializedProperty ScrollSnapAxis;
        private SerializedProperty VerticalPageAlignment;
        private SerializedProperty HorizontalPageAlignment;

        private bool snapAreaSizeChanged;

        private void OnEnable()
        {
            Content = serializedObject.FindProperty("Content");
            SnapSpeed = serializedObject.FindProperty("SnapSpeed");
            SnapAreaSize = serializedObject.FindProperty("SnapAreaSize");
            OnPageChanged = serializedObject.FindProperty("OnPageChanged");
            ScrollSnapAxis = serializedObject.FindProperty("ScrollSnapAxis");
            VerticalPageAlignment = serializedObject.FindProperty("VerticalPageAlignment");
            HorizontalPageAlignment = serializedObject.FindProperty("HorizontalPageAlignment");
        }

        [MenuItem("GameObject/UI/Scroll Snap/Infinite Scroll Snap", false)]
        private static void AddScrollSnap()
        {
            EditorUtilities.CreateInCanvas<ScrollSnapInfinite>(ScrollSnapInfiniteFileName, (canvas, target) => {
                target.SnapAreaSize = 600;
                var targetTransform = target.transform as RectTransform;
                var canvasTransform = canvas.transform as RectTransform;

                if (targetTransform == null || canvasTransform == null) return;
                
                targetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasTransform.rect.width);
                targetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasTransform.rect.height);
                targetTransform.anchoredPosition = new Vector2(0, 0);
            });
        }

        public override void OnInspectorGUI()
        {
            ScrollSnapInfinite scrollSnap = target as ScrollSnapInfinite;
            if (scrollSnap == null) return;

            GUI.enabled = !Application.isPlaying;
            bool isHorizontal = ((RectTransform.Axis)ScrollSnapAxis.enumValueIndex == RectTransform.Axis.Horizontal);

            serializedObject.Update();
            
            EditorGUILayout.PropertyField(Content, ContentLabel); 
            EditorGUILayout.PropertyField(ScrollSnapAxis, ScrollSnapAxisLabel);

            if (isHorizontal)
            {
                EditorGUILayout.PropertyField(HorizontalPageAlignment, HorizontalPageStartPositionLabel);
            }
            else
            {
                EditorGUILayout.PropertyField(VerticalPageAlignment, VerticalPageStartPositionLabel);
            }
            GUI.enabled = true;

            bool layoutChanged = GUI.changed;

            ValidateVariables(scrollSnap, isHorizontal);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(SnapAreaSize, SnapAreaSizeLabel);
            snapAreaSizeChanged = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(SnapSpeed, SnapSpeedLabel);
            
            if (GUILayout.Button("Add Page"))
            {
                scrollSnap.AddPage();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(OnPageChanged);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            if (GUI.changed)
            {
                if (layoutChanged) 
                {
                    snapAreaSizeChanged = true;
                    scrollSnap.ResetScrollSnapUI();
                }

                EditorUtility.SetDirty(scrollSnap);
            }
        }

        private void ValidateVariables(ScrollSnapInfinite scrollSnap, bool isHorizontal)
        {
            float pageSize = isHorizontal ? scrollSnap.PageWidth : scrollSnap.PageHeight;

            if (pageSize < SnapAreaSize.floatValue)
            {
                if (snapAreaSizeChanged)
                {
                    SnapAreaSize.floatValue = Mathf.Clamp(SnapAreaSize.floatValue, 0, pageSize);
                }
            }
        }
    }
}
