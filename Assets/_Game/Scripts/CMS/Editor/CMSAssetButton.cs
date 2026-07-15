using System.IO;
using Game.CMS.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.CMS.Editor
{
    public static class CMSAssetButton
    {
        [MenuItem("Assets/CMS/Create CMSAsset", priority = 1)]
        private static void CreateCMSEntityAsset()
        {
            string folderPath = GetSelectedFolderPath();
            var instance = ScriptableObject.CreateInstance<CMSAsset>();

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(folderPath, "CMSAsset.asset").Replace('\\', '/'));

            AssetDatabase.CreateAsset(instance, assetPath);
            instance.PingEntity();
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();

            Object createdAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            Selection.activeObject = createdAsset;
            EditorGUIUtility.PingObject(createdAsset);
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
