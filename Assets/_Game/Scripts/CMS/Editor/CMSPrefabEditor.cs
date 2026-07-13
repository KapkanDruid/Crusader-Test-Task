using System;
using Game.CMS.Runtime;
using UnityEditor;
using UnityEngine;

namespace Game.CMS.Editor
{
    [CustomEditor(typeof(CMSPrefab))]
    public sealed class CMSPrefabEditor : UnityEditor.Editor, ICMSEntityEditor
    {
        private SerializedProperty _componentsProperty;
        private SerializedProperty _entityIdProperty;
        private string _searchString = string.Empty;
        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            _componentsProperty = serializedObject.FindProperty("Components");
            _entityIdProperty = serializedObject.FindProperty("entityId");
            RefreshEntityId(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawEntityInfoSection();
            DrawComponentsSection();
            serializedObject.ApplyModifiedProperties();
        }

        internal static void RefreshAllEntityIds()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Game/Resources/CMS" });
            foreach (string prefabGuid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                CMSPrefab cmsPrefab = prefab != null ? prefab.GetComponent<CMSPrefab>() : null;
                if (cmsPrefab == null)
                    continue;

                var serializedPrefab = new SerializedObject(cmsPrefab);
                RefreshEntityId(serializedPrefab);
            }
        }

        private static void RefreshEntityId(SerializedObject serializedPrefab)
        {
            string assetPath = AssetDatabase.GetAssetPath(serializedPrefab.targetObject);
            const string resourcesPrefix = "Assets/_Game/Resources/";
            const string cmsPrefix = resourcesPrefix + "CMS/";
            if (!assetPath.StartsWith(cmsPrefix, StringComparison.Ordinal) ||
                !assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                return;

            string entityId = assetPath.Substring(resourcesPrefix.Length);
            entityId = entityId.Substring(0, entityId.Length - ".prefab".Length);

            serializedPrefab.Update();
            SerializedProperty entityIdProperty = serializedPrefab.FindProperty("entityId");
            if (entityIdProperty.stringValue == entityId)
                return;

            entityIdProperty.stringValue = entityId;
            serializedPrefab.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(serializedPrefab.targetObject);
        }

        private void DrawEntityInfoSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Entity ID", EditorStyles.boldLabel,
                GUILayout.Width(EditorGUIUtility.labelWidth - 4f));

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.TextField(_entityIdProperty.stringValue);

            if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(60f)))
                RefreshEntityId(serializedObject);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Components Count", _componentsProperty.arraySize.ToString());
            _searchString = EditorGUILayout.TextField(_searchString, EditorStyles.toolbarSearchField);
            EditorGUILayout.EndVertical();
        }

        private void DrawComponentsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(false));

            for (int index = 0; index < _componentsProperty.arraySize; index++)
            {
                SerializedProperty element = _componentsProperty.GetArrayElementAtIndex(index);
                object value = element.managedReferenceValue;
                if (value == null)
                    continue;

                string typeName = value.GetType().Name;
                if (!string.IsNullOrEmpty(_searchString) &&
                    typeName.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                DrawComponent(element, typeName, index);
            }

            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Add Component", EditorStyles.miniButton))
                ShowAddComponentMenu();
            EditorGUILayout.EndVertical();
        }

        private void DrawComponent(SerializedProperty element, string typeName, int index)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, typeName, true);

            using (new EditorGUI.DisabledScope(index == 0))
                if (GUILayout.Button("↑", GUILayout.Width(22f)))
                    _componentsProperty.MoveArrayElement(index, index - 1);

            using (new EditorGUI.DisabledScope(index == _componentsProperty.arraySize - 1))
                if (GUILayout.Button("↓", GUILayout.Width(22f)))
                    _componentsProperty.MoveArrayElement(index, index + 1);

            if (GUILayout.Button("×", GUILayout.Width(22f)))
                _componentsProperty.DeleteArrayElementAtIndex(index);

            EditorGUILayout.EndHorizontal();
            if (element.isExpanded)
                EditorGUILayout.PropertyField(element, GUIContent.none, true);
            EditorGUILayout.EndVertical();
        }

        private void ShowAddComponentMenu()
        {
            var window = EditorWindow.GetWindow<CMSComponentsWindow>(true, "Add CMS Component", true);
            window.Initialize(this);
            window.position = new Rect(
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                new Vector2(350f, 500f));
            window.ShowPopup();
        }

        public void AddComponent(Type componentType)
        {
            serializedObject.Update();
            int index = _componentsProperty.arraySize;
            _componentsProperty.arraySize++;
            _componentsProperty.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(componentType);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
