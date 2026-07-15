using System;
using System.Collections.Generic;
using System.Linq;
using Game.CMS.Runtime;
using UnityEditor;
using UnityEngine;

namespace Game.CMS.Editor
{
    public sealed class CMSComponentsWindow : EditorWindow
    {
        private sealed class ComponentFolder
        {
            public string DisplayName;
            public readonly Dictionary<string, ComponentFolder> Subfolders = new();
            public readonly List<Type> Components = new();
        }

        private readonly ComponentFolder _rootFolder = new();
        private readonly Dictionary<string, bool> _folderExpansionStates = new();
        private string _search = string.Empty;
        private Vector2 _scrollPosition;
        private ICMSEntityEditor _editor;

        public void Initialize(ICMSEntityEditor editor)
        {
            _editor = editor;
            _rootFolder.Subfolders.Clear();
            _rootFolder.Components.Clear();

            foreach (Type type in TypeCache.GetTypesDerivedFrom<CMSComponent>()
                         .Where(type => !type.IsAbstract && !type.IsInterface)
                         .OrderBy(type => type.Name))
            {
                AddComponentToFolder(type, type.Namespace ?? "Other");
            }
        }

        private void AddComponentToFolder(Type type, string namespaceName)
        {
            ComponentFolder folder = _rootFolder;
            if (!string.IsNullOrEmpty(namespaceName))
            {
                string folderName = namespaceName.Split('.').Last();
                if (!folder.Subfolders.TryGetValue(folderName, out ComponentFolder child))
                {
                    child = new ComponentFolder { DisplayName = folderName };
                    folder.Subfolders.Add(folderName, child);
                }

                folder = child;
            }

            folder.Components.Add(type);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _search = EditorGUILayout.TextField(_search, EditorStyles.toolbarSearchField);
            EditorGUILayout.EndHorizontal();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            if (string.IsNullOrWhiteSpace(_search))
            {
                DrawFolder(_rootFolder, 0, string.Empty);
            }
            else
            {
                foreach (Type type in GetAllComponents(_rootFolder)
                             .Where(type => type.Name.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0)
                             .OrderBy(type => type.Name))
                {
                    DrawComponentButton(type, 0);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private static IEnumerable<Type> GetAllComponents(ComponentFolder folder)
        {
            return folder.Components.Concat(folder.Subfolders.Values.SelectMany(GetAllComponents));
        }

        private void DrawFolder(ComponentFolder folder, int indentLevel, string parentKey)
        {
            foreach (ComponentFolder subfolder in folder.Subfolders.Values.OrderBy(value => value.DisplayName))
            {
                string folderKey = string.IsNullOrEmpty(parentKey)
                    ? subfolder.DisplayName
                    : $"{parentKey}.{subfolder.DisplayName}";

                _folderExpansionStates.TryAdd(folderKey, false);
                _folderExpansionStates[folderKey] = EditorGUILayout.Foldout(
                    _folderExpansionStates[folderKey], subfolder.DisplayName, true);

                if (_folderExpansionStates[folderKey])
                    DrawFolder(subfolder, indentLevel + 1, folderKey);
            }

            foreach (Type type in folder.Components.OrderBy(type => type.Name))
                DrawComponentButton(type, indentLevel);
        }

        private void DrawComponentButton(Type type, int indentLevel)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel * 8f);
            if (GUILayout.Button(type.Name, EditorStyles.miniButton, GUILayout.Height(20f)))
            {
                _editor.AddComponent(type);
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
