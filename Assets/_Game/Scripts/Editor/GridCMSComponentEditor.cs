using Game.Runtime.Data.CMSComponents.Config;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomPropertyDrawer(typeof(GridCMSComponent.GridData))]
    public sealed class GridCMSComponentEditor : PropertyDrawer
    {
        private const float LineHeight = 18f;
        private const float VerticalSpacing = 2f;
        private const float SectionSpacing = 10f;
        private const float GenerateButtonHeight = 25f;
        private const float ActionButtonHeight = 30f;
        private const float HelpBoxHeight = 40f;
        private const float CellSize = 40f;
        private const float CellSpacing = 2f;

        private const string GridSizeWarning =
            "The grid size is changed forcibly because the saved pattern does not fit into the specified grid size";
        private const string UnsavedChangesWarning = "Changes made are not saved!";

        private sealed class GridEditorState
        {
            public Vector2Int LocalGridSize;
            public HashSet<Vector2Int> LocalGridPattern = new();
            public int MaxX;
            public int MaxY;
            public bool SizeModifiedForced;
            public float LastWidth = 300f;
        }

        private readonly Dictionary<string, GridEditorState> _states = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty patternProperty = property.FindPropertyRelative("gridPattern");
            SerializedProperty sizeProperty = property.FindPropertyRelative("gridSize");

            if (patternProperty == null || sizeProperty == null)
            {
                EditorGUI.HelpBox(position, "Grid CMS data is not serialized correctly.", MessageType.Error);
                return;
            }

            GridEditorState state = GetState(property, patternProperty, sizeProperty);
            state.LastWidth = Mathf.Max(120f, position.width);
            List<Vector2Int> savedPattern = ReadPattern(patternProperty);
            bool changesSaved = ArePatternsEqual(state.LocalGridPattern, savedPattern);

            EditorGUI.BeginProperty(position, label, property);

            float y = position.y;

            if (state.SizeModifiedForced)
                EditorGUI.HelpBox(TakeRect(position, ref y, HelpBoxHeight), GridSizeWarning, MessageType.Warning);

            if (!changesSaved)
                EditorGUI.HelpBox(TakeRect(position, ref y, HelpBoxHeight), UnsavedChangesWarning, MessageType.Warning);

            state.LocalGridSize = EditorGUI.Vector2IntField(
                TakeRect(position, ref y, LineHeight), "Grid Size", state.LocalGridSize);

            y += SectionSpacing;

            Rect generateRect = CenteredRect(TakeRect(position, ref y, GenerateButtonHeight), 100f);
            if (GUI.Button(generateRect, "Generate Grid"))
                GenerateGrid(state, savedPattern, false);

            y += SectionSpacing;

            if (savedPattern.Count > 0 || state.LocalGridPattern.Count > 0)
            {
                Rect actionsRect = TakeRect(position, ref y, ActionButtonHeight);
                DrawActionButtons(actionsRect, patternProperty, sizeProperty, state, savedPattern, changesSaved);
            }

            y += SectionSpacing;

            savedPattern = ReadPattern(patternProperty);
            changesSaved = ArePatternsEqual(state.LocalGridPattern, savedPattern);

            if (savedPattern.Count > 0)
            {
                DrawPatternText(position, ref y, "Saved pattern", savedPattern, Color.white);
            }

            y += 4f;

            if (state.LocalGridPattern.Count > 0)
            {
                Color localColor = changesSaved ? Color.green : Color.yellow;
                DrawPatternText(position, ref y, "Local pattern", state.LocalGridPattern, localColor);
            }

            y += SectionSpacing;
            DrawGrid(position, ref y, state);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty patternProperty = property.FindPropertyRelative("gridPattern");
            SerializedProperty sizeProperty = property.FindPropertyRelative("gridSize");

            if (patternProperty == null || sizeProperty == null)
                return HelpBoxHeight;

            GridEditorState state = GetState(property, patternProperty, sizeProperty);
            List<Vector2Int> savedPattern = ReadPattern(patternProperty);
            bool changesSaved = ArePatternsEqual(state.LocalGridPattern, savedPattern);
            float availableWidth = state.LastWidth;

            float height = 0f;

            if (state.SizeModifiedForced)
                height += HelpBoxHeight + VerticalSpacing;

            if (!changesSaved)
                height += HelpBoxHeight + VerticalSpacing;

            height += LineHeight + VerticalSpacing;
            height += SectionSpacing;
            height += GenerateButtonHeight + VerticalSpacing;
            height += SectionSpacing;

            if (savedPattern.Count > 0 || state.LocalGridPattern.Count > 0)
                height += ActionButtonHeight + VerticalSpacing;

            height += SectionSpacing;

            if (savedPattern.Count > 0)
                height += GetPatternTextHeight(savedPattern, availableWidth);

            height += 4f;

            if (state.LocalGridPattern.Count > 0)
                height += GetPatternTextHeight(state.LocalGridPattern, availableWidth);

            height += SectionSpacing;

            int rowCount = state.MaxY * 2 + 1;
            height += rowCount * CellSize + Mathf.Max(0, rowCount - 1) * CellSpacing;

            return height;
        }

        private GridEditorState GetState(
            SerializedProperty property,
            SerializedProperty patternProperty,
            SerializedProperty sizeProperty)
        {
            int targetId = property.serializedObject.targetObject != null
                ? property.serializedObject.targetObject.GetInstanceID()
                : 0;
            string key = $"{targetId}:{property.propertyPath}";

            if (_states.TryGetValue(key, out GridEditorState state))
                return state;

            state = new GridEditorState
            {
                LocalGridSize = sizeProperty.vector2IntValue
            };

            GenerateGrid(state, ReadPattern(patternProperty), true);
            _states.Add(key, state);
            return state;
        }

        private static void GenerateGrid(
            GridEditorState state,
            IReadOnlyCollection<Vector2Int> savedPattern,
            bool firstCall)
        {
            state.SizeModifiedForced = false;

            foreach (Vector2Int gridPosition in savedPattern)
            {
                int suitableWidth = Mathf.Abs(gridPosition.x) * 2 + 1;
                if (suitableWidth > state.LocalGridSize.x)
                {
                    state.LocalGridSize = new Vector2Int(suitableWidth, state.LocalGridSize.y);
                    if (!firstCall)
                        state.SizeModifiedForced = true;
                }

                int suitableHeight = Mathf.Abs(gridPosition.y) * 2 + 1;
                if (suitableHeight > state.LocalGridSize.y)
                {
                    state.LocalGridSize = new Vector2Int(state.LocalGridSize.x, suitableHeight);
                    if (!firstCall)
                        state.SizeModifiedForced = true;
                }
            }

            state.MaxX = Mathf.Max(0, state.LocalGridSize.x / 2);
            state.MaxY = Mathf.Max(0, state.LocalGridSize.y / 2);
            state.LocalGridPattern = new HashSet<Vector2Int>(savedPattern);
        }

        private static void DrawActionButtons(
            Rect position,
            SerializedProperty patternProperty,
            SerializedProperty sizeProperty,
            GridEditorState state,
            IReadOnlyCollection<Vector2Int> savedPattern,
            bool changesSaved)
        {
            bool showClear = savedPattern.Count > 0;
            bool showSave = state.LocalGridPattern.Count > 0;
            float gap = showClear && showSave ? 4f : 0f;
            float buttonWidth = Mathf.Min(200f, (position.width - gap) / (showClear && showSave ? 2f : 1f));
            float totalWidth = buttonWidth * (showClear && showSave ? 2f : 1f) + gap;
            float x = position.x + (position.width - totalWidth) * 0.5f;
            Color previousColor = GUI.backgroundColor;

            if (showClear)
            {
                GUI.backgroundColor = Color.red;
                if (GUI.Button(new Rect(x, position.y, buttonWidth, position.height), "Clear saved pattern"))
                {
                    patternProperty.ClearArray();
                    GUI.changed = true;
                }

                x += buttonWidth + gap;
            }

            if (showSave)
            {
                GUI.backgroundColor = changesSaved ? Color.white : Color.yellow;
                if (GUI.Button(new Rect(x, position.y, buttonWidth, position.height), "Save current pattern"))
                {
                    SavePattern(patternProperty, sizeProperty, state);
                    GUI.changed = true;
                }
            }

            GUI.backgroundColor = previousColor;
        }

        private static void SavePattern(
            SerializedProperty patternProperty,
            SerializedProperty sizeProperty,
            GridEditorState state)
        {
            List<Vector2Int> sortedPattern = state.LocalGridPattern
                .OrderBy(position => position.x)
                .ThenBy(position => position.y)
                .ToList();

            patternProperty.ClearArray();
            for (int index = 0; index < sortedPattern.Count; index++)
            {
                patternProperty.InsertArrayElementAtIndex(index);
                patternProperty.GetArrayElementAtIndex(index).vector2IntValue = sortedPattern[index];
            }

            sizeProperty.vector2IntValue = state.LocalGridSize;
        }

        private static void DrawPatternText(
            Rect position,
            ref float y,
            string title,
            IEnumerable<Vector2Int> pattern,
            Color color)
        {
            List<Vector2Int> positions = pattern.ToList();
            GUIStyle titleStyle = CreateTextStyle(14, color);
            GUIStyle positionsStyle = CreateTextStyle(12, color);

            EditorGUI.LabelField(
                TakeRect(position, ref y, LineHeight),
                $"{title}: {positions.Count} positions",
                titleStyle);

            string positionsText = string.Join(" ", positions);
            float textHeight = Mathf.Max(LineHeight, positionsStyle.CalcHeight(
                new GUIContent(positionsText), position.width));
            EditorGUI.LabelField(TakeRect(position, ref y, textHeight), positionsText, positionsStyle);
        }

        private static void DrawGrid(Rect position, ref float y, GridEditorState state)
        {
            int columnCount = state.MaxX * 2 + 1;
            float gridWidth = columnCount * CellSize + Mathf.Max(0, columnCount - 1) * CellSpacing;
            float startX = position.x + Mathf.Max(0f, (position.width - gridWidth) * 0.5f);
            Color previousColor = GUI.backgroundColor;

            for (int row = -state.MaxY; row <= state.MaxY; row++)
            {
                float x = startX;
                for (int column = -state.MaxX; column <= state.MaxX; column++)
                {
                    Vector2Int currentPosition = new(column, -row);
                    GUI.backgroundColor = state.LocalGridPattern.Contains(currentPosition)
                        ? Color.green
                        : Color.white;

                    if (GUI.Button(new Rect(x, y, CellSize, CellSize), $"{column},{-row}"))
                    {
                        if (!state.LocalGridPattern.Add(currentPosition))
                            state.LocalGridPattern.Remove(currentPosition);
                    }

                    x += CellSize + CellSpacing;
                }

                y += CellSize;
                if (row < state.MaxY)
                    y += CellSpacing;
            }

            GUI.backgroundColor = previousColor;
        }

        private static List<Vector2Int> ReadPattern(SerializedProperty patternProperty)
        {
            var pattern = new List<Vector2Int>(patternProperty.arraySize);
            for (int index = 0; index < patternProperty.arraySize; index++)
                pattern.Add(patternProperty.GetArrayElementAtIndex(index).vector2IntValue);

            return pattern;
        }

        private static bool ArePatternsEqual(
            IReadOnlyCollection<Vector2Int> localPattern,
            IReadOnlyCollection<Vector2Int> savedPattern)
        {
            if (localPattern.Count != savedPattern.Count)
                return false;

            var localSet = localPattern as HashSet<Vector2Int> ?? new HashSet<Vector2Int>(localPattern);
            return savedPattern.All(localSet.Contains);
        }

        private static float GetPatternTextHeight(IEnumerable<Vector2Int> pattern, float width)
        {
            List<Vector2Int> positions = pattern.ToList();
            GUIStyle positionsStyle = CreateTextStyle(12, Color.white);
            float positionsHeight = Mathf.Max(LineHeight, positionsStyle.CalcHeight(
                new GUIContent(string.Join(" ", positions)), width));

            return LineHeight + VerticalSpacing + positionsHeight + VerticalSpacing;
        }

        private static GUIStyle CreateTextStyle(int fontSize, Color color)
        {
            var style = new GUIStyle(EditorStyles.label)
            {
                fontSize = fontSize,
                wordWrap = true
            };
            style.normal.textColor = color;
            return style;
        }

        private static Rect TakeRect(Rect position, ref float y, float height)
        {
            var rect = new Rect(position.x, y, position.width, height);
            y += height + VerticalSpacing;
            return rect;
        }

        private static Rect CenteredRect(Rect position, float width)
        {
            return new Rect(
                position.x + Mathf.Max(0f, (position.width - width) * 0.5f),
                position.y,
                Mathf.Min(width, position.width),
                position.height);
        }
    }
}
