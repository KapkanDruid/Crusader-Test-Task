using Game.CMS.Runtime;
using Game.Runtime.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class SpeedButtonsViewConfigComponent : CMSComponent
    {
        [SerializeField] private List<float> _speedValues = new();
        [SerializeField] private SpeedButtonView _speedButtonPrefab;

        public IReadOnlyList<float> SpeedValues => _speedValues;
        public SpeedButtonView SpeedButtonPrefab => _speedButtonPrefab;
    }
}
