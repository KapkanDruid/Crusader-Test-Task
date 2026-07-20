using Game.Runtime.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Items
{
    public class ItemTriggerCellView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void Setup(Vector2Int gridPosition, int cellSize)
        {
            name = $"ItemTrigger {gridPosition}";

            _image.SetAlpha(0);
            _image.rectTransform.SetAsFirstSibling();
            _image.rectTransform.localPosition = Vector3.zero;
            _image.rectTransform.localScale = Vector3.one;
            _image.rectTransform.sizeDelta = Vector2.one * cellSize;
            _image.rectTransform.anchoredPosition = gridPosition * cellSize;
        }
    }
}
