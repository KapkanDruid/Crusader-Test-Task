using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Utils
{
    public static class GraphicExtensions
    {
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
        }

        public static bool IsPointerInside(this Graphic graphic, Vector2 pointerPoint)
        {
            Camera camera = graphic.canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : graphic.canvas.worldCamera;
            return RectTransformUtility.RectangleContainsScreenPoint(
                graphic.rectTransform,
                pointerPoint,
                camera
                );
        }
    }
}
