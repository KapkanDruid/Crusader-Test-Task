using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Resources
{
    public class ResourceGroupElement : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private CanvasGroup _canvasGroup;

        public RectTransform RectTransform => _rectTransform;
        public CanvasGroup CanvasGroup => _canvasGroup;

        public void Setup(Sprite sprite, int count)
        {
            _image.sprite = sprite;
            SetCount(count);
        }

        public void SetCount(int count)
        {
            _countText.text = count.ToString();
        }
    }
}
