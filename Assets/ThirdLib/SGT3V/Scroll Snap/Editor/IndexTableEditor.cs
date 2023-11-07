using UnityEditor;
using UnityEngine;
using SGT3V.Common;
using UnityEngine.UI;
using SGT3V.Common.UI;

namespace SGT3V.ScrollSnap
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(IndexTable))]
    public class IndexTableEditor : Editor
    {
        private const string TOGGLE_PREFAB_NAME = "Toggle";
        private const string INDEX_TABLE_PREFAB_NAME = "Index Table";

        private readonly GUIContent SnapScrollLabel = new GUIContent("Scroll Snap", "The parent Scroll Snap component, that the Index Table will work with.");
        private readonly GUIContent ToggleSizeLabel = new GUIContent("Toggle Size", "The size of the Toggle by height.");
        private readonly GUIContent TogglePaddingLabel = new GUIContent("Toggle Padding", "The padding between the buttons of the Index Table.");

        private SerializedProperty ScrollSnap; 
        private SerializedProperty ToggleSize;
        private SerializedProperty TogglePadding;
        
        private bool firstTime = true;

        private void OnEnable()
        {
            ScrollSnap = serializedObject.FindProperty("ScrollSnap");
            ToggleSize = serializedObject.FindProperty("ToggleSize");
            TogglePadding = serializedObject.FindProperty("TogglePadding");
        }

        [MenuItem("GameObject/UI/Scroll Snap/Index Table")]
        private static void AddIndexTable()
        {
            EditorUtilities.CreateInCanvas<IndexTable>(INDEX_TABLE_PREFAB_NAME, (canvas, table) => {
                table.ToggleSize = 30;
                table.TogglePadding = 5;
                table.ScrollSnap = table.GetComponentInParent<ScrollSnap>();
                
                RectTransform rectTransform = table.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = Vector2.zero;
                
                EditorUtility.SetDirty(canvas);
            });
        }

        [MenuItem("GameObject/UI/Scroll Snap/Index Table", true)]
        private static bool ValidateAddIndexTable()
        {
            var active = Selection.activeGameObject;
            if (active)
            {
                return active.GetComponent<ScrollSnap>() != null || active.GetComponent<ScrollSnapInfinite>() != null;
            }
            return false;
        }

        public override void OnInspectorGUI()
        {
            var indexTable = target as IndexTable;
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(ScrollSnap, SnapScrollLabel);
            if (EditorGUI.EndChangeCheck() || firstTime)
            {
                var scrollSnap = ScrollSnap.objectReferenceValue as ScrollSnap;
                OnTableCreated(scrollSnap, indexTable);
                OnToggleSizeChanged(indexTable);
                firstTime = false;
            }

            if(ScrollSnap.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(ToggleSize, ToggleSizeLabel);
                if (EditorGUI.EndChangeCheck())
                {
                    OnToggleSizeChanged(indexTable);
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(TogglePadding, TogglePaddingLabel);
                if (EditorGUI.EndChangeCheck())
                {
                    OnToggleSizeChanged(indexTable);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnTableCreated(ScrollSnap scrollSnap, Component table)
        {
            table.transform.DestroyChildren();

            if (scrollSnap == null) return;

            var pageCount = scrollSnap.Content.childCount;
            (table.transform as RectTransform)?.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pageCount * ToggleSize.floatValue);

            var toggleGroup = table.GetComponent<ToggleGroup>();

            for (var i = 0; i < pageCount; i++)
            {
                var toggle = EditorUtilities.LoadAndInstantiate<Toggle>(TOGGLE_PREFAB_NAME, table.transform);
                (toggle.transform as RectTransform).anchoredPosition = new Vector2(i * 50, 0);
                toggle.group = toggleGroup;
                toggle.isOn = i == 0;
            }
        }

        private void OnToggleSizeChanged(Component table)
        {
            var toggles = table.GetComponentsInChildren<Toggle>();

            for (var i = 0; i < toggles.Length; i++)
            {
                var rect = (toggles[i].transform as RectTransform);
                if (rect != null)
                {
                    rect.anchoredPosition = new Vector2(i * (ToggleSize.floatValue + TogglePadding.floatValue), 0);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ToggleSize.floatValue);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ToggleSize.floatValue);
                }
            }

            (table.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (toggles.Length) * (ToggleSize.floatValue + TogglePadding.floatValue) - TogglePadding.floatValue);
            (table.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ToggleSize.floatValue);
        }
    }
}
