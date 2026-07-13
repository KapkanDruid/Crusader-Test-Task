using System.IO;
using Game.CMS.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.CMS.Editor
{
    public static class CMSPrefabButton
    {
        [MenuItem("Assets/CMS/Create CMSPrefab", priority = 0)]
        private static void CreateCMSEntityPrefab()
        {
            string folderPath = GetSelectedFolderPath();
            var instance = new GameObject("CMSPrefab");
            instance.AddComponent<CMSPrefab>();

            string prefabPath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(folderPath, "CMSPrefab.prefab").Replace('\\', '/'));

            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Object.DestroyImmediate(instance);

            Object createdPrefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            Selection.activeObject = createdPrefab;
            EditorGUIUtility.PingObject(createdPrefab);
        }

        private static string GetSelectedFolderPath()
        {
            foreach (Object selectedObject in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                if (string.IsNullOrEmpty(assetPath))
                    continue;

                return File.Exists(assetPath) ? Path.GetDirectoryName(assetPath) ?? "Assets" : assetPath;
            }

            return "Assets";
        }
    }
}
