using System;
using Game.CMS.Runtime;
using UnityEditor;
using UnityEngine;

namespace Game.CMS.Editor
{
    [CustomEditor(typeof(CMSAsset))]
    public sealed class CMSAssetEditor : UnityEditor.Editor, ICMSEntityEditor
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
            string[] assetGuids = AssetDatabase.FindAssets("t:CMSAsset", new[] { "Assets/_Game/Resources/CMS" });
            foreach (string assetGuid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuid);
                CMSAsset cmsAsset = AssetDatabase.LoadAssetAtPath<CMSAsset>(path);
                if (cmsAsset == null)
                    continue;

                var serializedAsset = new SerializedObject(cmsAsset);
                RefreshEntityId(serializedAsset);
            }
        }

        private static void RefreshEntityId(SerializedObject serializedAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(serializedAsset.targetObject);
            const string resourcesPrefix = "Assets/_Game/Resources/";
            const string cmsPrefix = resourcesPrefix + "CMS/";
            if (!assetPath.StartsWith(cmsPrefix, StringComparison.Ordinal) ||
                !assetPath.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                return;

            if (serializedAsset.targetObject is not CMSAsset cmsAsset)
                return;

            cmsAsset.PingEntity();
            serializedAsset.Update();
            EditorUtility.SetDirty(cmsAsset);
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

                if (DrawComponent(element, typeName, index))
                    break;
            }

            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Add Component", EditorStyles.miniButton))
                ShowAddComponentMenu();
            EditorGUILayout.EndVertical();
        }

        private bool DrawComponent(SerializedProperty element, string typeName, int index)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(12f);
            bool structureChanged = false;
            element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, typeName, true);

            using (new EditorGUI.DisabledScope(index == 0))
                if (GUILayout.Button("↑", GUILayout.Width(20f)))
                {
                    _componentsProperty.MoveArrayElement(index, index - 1);
                    structureChanged = true;
                }

            using (new EditorGUI.DisabledScope(index == _componentsProperty.arraySize - 1))
                if (GUILayout.Button("↓", GUILayout.Width(20f)))
                {
                    _componentsProperty.MoveArrayElement(index, index + 1);
                    structureChanged = true;
                }

            if (GUILayout.Button("×", EditorStyles.miniButtonRight, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                _componentsProperty.DeleteArrayElementAtIndex(index);
                structureChanged = true;
            }

            EditorGUILayout.EndHorizontal();

            if (structureChanged)
            {
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
                return true;
            }

            if (element.isExpanded)
            {
                EditorGUILayout.Space(2f);
                EditorGUI.indentLevel++;
                DrawAllProperties(element);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(2f);
            EditorGUILayout.EndVertical();
            return false;
        }

        private static void DrawAllProperties(SerializedProperty property)
        {
            SerializedProperty iterator = property.Copy();
            SerializedProperty end = iterator.GetEndProperty();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
            {
                enterChildren = false;
                if (iterator.name == "data")
                    continue;

                EditorGUILayout.PropertyField(iterator, true);
            }
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
