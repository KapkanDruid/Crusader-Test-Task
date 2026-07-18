using UnityEngine;

namespace Game.Runtime.Utils
{
    public static class RectTransformExtensions
    {
        public static bool IsTargetInsideRectTransform(this RectTransform rectTransform, Transform targetTransform)
        {
            return rectTransform.rect.Contains(rectTransform.transform.InverseTransformPoint(targetTransform.position));
        }
    }
}
