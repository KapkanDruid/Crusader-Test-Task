using Game.CMS.Runtime.Utils.Helpers;
using System;
using UnityEngine;

namespace Game.CMS.Runtime.Utils
{
    public static class LogUtil
    {
        [HideInCallstack]
        public static void Log(string caller, string logString = null, Color textColor = default,
            LogType logType = LogType.Log, bool editorOnly = false)
        {
#if !UNITY_EDITOR
            if (editorOnly)
                return;
#endif

#if UNITY_EDITOR
            var color = ColorUtility.ToHtmlStringRGB(textColor == default ? Color.white : textColor);
            logString = $"<b><color=#{color}>[{caller}]</b>. {(string.IsNullOrEmpty(logString) ? string.Empty : logString)}</color>";
#else
            logString = $"[{caller}]. {(string.IsNullOrEmpty(logString) ? string.Empty : logString)}";
#endif

            Debug.unityLogger.Log(logType, logString);
        }

        [HideInCallstack]
        public static void Log(object caller, string logString = null, Color textColor = default,
            LogType logType = LogType.Log, bool editorOnly = false)
        {
            Log(caller.GetType().Name, logString, textColor, logType, editorOnly);
        }

        [HideInCallstack]
        public static void Log(object caller, string logString, object objectHashToColor,
            LogType logType = LogType.Log, bool editorOnly = false)
        {
            Log(caller.GetType().Name, logString, ColorHelper.IntToBrightColor(objectHashToColor.GetHashCode()), logType, editorOnly);
        }

        [HideInCallstack]
        public static void LogWarning(string caller, string logString = null, bool editorOnly = false)
        {
            Log(caller, logString, Color.yellow, LogType.Warning, editorOnly);
        }

        [HideInCallstack]
        public static void LogWarning(Type caller, string logString = null, bool editorOnly = false)
        {
            Log(caller.Name, logString, Color.yellow, LogType.Warning, editorOnly);
        }

        [HideInCallstack]
        public static void LogWarning(object caller, string logString = null, bool editorOnly = false)
        {
            Log(caller.GetType().Name, logString, Color.yellow, LogType.Warning, editorOnly);
        }

        [HideInCallstack]
        public static void LogError(string caller, string logString = null, bool editorOnly = false)
        {
            Log(caller, logString, textColor: new Color(1f, 0.4f, 0.4f), LogType.Error, editorOnly);
        }

        [HideInCallstack]
        public static void LogError(Type caller, string logString = null, bool editorOnly = false)
        {
            Log(caller.Name, logString, textColor: new Color(1f, 0.4f, 0.4f), LogType.Error, editorOnly);
        }

        [HideInCallstack]
        public static void LogError(object caller, string logString = null, bool editorOnly = false)
        {
            Log(caller.GetType().Name, logString, textColor: new Color(1f, 0.4f, 0.4f), LogType.Error, editorOnly);
        }
    }
}
